using Autofac;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceHunter.Business.Product.Concrete;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Options;
using PriceHunter.Contract.Consumer.Notification;
using PriceHunter.Contract.Consumer.Parser;
using PriceHunter.Model.Product;
using PriceHunter.Model.Supplier;
using PriceHunter.Parser;
using PriceHunter.Parser.Models;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model;
using System.Diagnostics;

namespace PriceHunter.Consumer.Parser.Consumers
{
    public class ParserConsumer : IConsumer<SendParserCommand>
    {
        private readonly IGenericRepository<ProductSupplierInfoMapping> _productSupplierInfoMappingRepository;
        private readonly IGenericRepository<ProductPriceHistory> _productPriceHistoryRepository;
        private readonly IGenericRepository<SupplierPriceParseScript> _supplierPriceParseScriptRepository;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RabbitMqOption _rabbitMqOptions;
        private readonly ILogger<ProductService> _logger;
        private readonly ILifetimeScope _lifetimeScope;

        public ParserConsumer(
            IGenericRepository<ProductSupplierInfoMapping> productSupplierInfoMappingRepository,
            IGenericRepository<ProductPriceHistory> productPriceHistoryRepository,
            IGenericRepository<SupplierPriceParseScript> supplierPriceParseScriptRepository,
            ISendEndpointProvider sendEndpointProvider,
            IOptions<RabbitMqOption> rabbitMqOptions,
            ILogger<ProductService> logger,
            ILifetimeScope lifetimeScope
        )
        {
            _productSupplierInfoMappingRepository = productSupplierInfoMappingRepository;
            _productPriceHistoryRepository = productPriceHistoryRepository;
            _supplierPriceParseScriptRepository = supplierPriceParseScriptRepository;
            _rabbitMqOptions = rabbitMqOptions.Value;
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
            _lifetimeScope = lifetimeScope;
        }
        public async Task Consume(ConsumeContext<SendParserCommand> context)
        {
            try
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("----");
                    Console.WriteLine($"Parser Consumer - ProductId:{context.Message.ProductId} - SupplierId:{context.Message.SupplierId} - Url:{context.Message.Url} - EunmMapping:{context.Message.EnumMapping} - RequestTime:{context.Message.RequestTime} - ProcessTime:{DateTime.Now}");
                }

                var mapping = await _productSupplierInfoMappingRepository.FindOneAsync(p => p.ProductId == context.Message.ProductId && p.SupplierId == context.Message.SupplierId && p.IsDeleted == false, context.CancellationToken);
                if (mapping == null)
                {
                    _logger.LogInformation(string.Format($"{Resource.NotFound(Entities.Product)} - ProductId: {context.Message.ProductId} - SupplierId:{context.Message.SupplierId}"));
                    return;
                }

                var parseScripts = _supplierPriceParseScriptRepository.Find(p => p.SupplierId == context.Message.SupplierId && p.IsDeleted == false).ToList();
                if (parseScripts == null || !parseScripts.Any())
                {
                    _logger.LogInformation(string.Format($"{Resource.NotFound(Entities.SupplierPriceParseScript)} - ProductId: {context.Message.ProductId} - SupplierId:{context.Message.SupplierId}"));
                    return;
                }

                var lastPriceHistoryItem = _productPriceHistoryRepository.Find(p =>
                p.ProductId == context.Message.ProductId &&
                p.SupplierId == context.Message.SupplierId &&
                p.IsDeleted == false)
                    .OrderByDescending(p => p.CreatedBy)
                    .FirstOrDefault();

                if (lastPriceHistoryItem != null && lastPriceHistoryItem.CreatedOn >= context.Message.RequestTime)
                {
                    _logger.LogInformation(string.Format($"ProductId: {context.Message.ProductId} - SupplierId:{context.Message.SupplierId} - RequestTime:{context.Message.RequestTime} - LastPriceDate:{lastPriceHistoryItem.CreatedOn}; already has new price data."));
                    return;
                }

                var supplierType = (SupplierType)context.Message.EnumMapping;
                var parsedResponse = new ParseResponse();

                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    var parser = scope.ResolveKeyed<IParser>(supplierType);
                    parsedResponse = await parser.ParseAsync(context.Message.Url, parseScripts.Select(p => p.Script).ToList());
                }

                if (!parsedResponse.IsSuccess)
                {
                    var errorMessage = string.Join(",", parsedResponse.ErrorMessages);
                    _logger.LogError(errorMessage);
                }

                var operationTime = DateTime.UtcNow;
                await _productPriceHistoryRepository.InsertAsync(new ProductPriceHistory
                {
                    ProductId = context.Message.ProductId,
                    SupplierId = context.Message.SupplierId,
                    Price = parsedResponse.Price,
                    Year = operationTime.Year,
                    Month = operationTime.Month,
                    Day = operationTime.Day,
                    Time = operationTime.TimeOfDay
                }, context.CancellationToken);

                if (lastPriceHistoryItem != null && parsedResponse.Price != null && lastPriceHistoryItem.Price > parsedResponse.Price)
                {
                    var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"{_rabbitMqOptions.RabbitMqUri}/{_rabbitMqOptions.NotificationQueue}"));
                    await endpoint.Send(new SendNotificationCommand
                    {
                        ProductId = context.Message.ProductId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}