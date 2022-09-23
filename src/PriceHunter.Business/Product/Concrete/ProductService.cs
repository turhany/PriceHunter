using AutoMapper;
using Filtery.Extensions;
using Filtery.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceHunter.Business.Product.Abstract;
using PriceHunter.Business.Product.Validator;
using PriceHunter.Cache.Abstract;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Options;
using PriceHunter.Common.Pager;
using PriceHunter.Common.Validation.Abstract;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Consumer.Parser;
using PriceHunter.Contract.Mappings.Filtery;
using PriceHunter.Contract.Service.Product;
using PriceHunter.Model.Product;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model;
using PriceHunter.Resources.Service;
using System.Dynamic;
using PriceHunter.Lock.Abstract;
using PriceHunter.Cache.Constants;
using Hangfire;

namespace PriceHunter.Business.Product.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.Product> _productRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> _productSupplierInfoMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.ProductPriceHistory> _productPriceHistoryRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> _userProductSupplierMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.Currency.Currency> _currencyRepository;
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RabbitMqOption _rabbitMqOptions;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
        IGenericRepository<PriceHunter.Model.Supplier.Supplier> supplierRepository,
        IGenericRepository<PriceHunter.Model.Product.Product> productRepository,
        IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> productSupplierInfoMappingRepository,
        IGenericRepository<PriceHunter.Model.Product.ProductPriceHistory> productPriceHistoryRepository,
        IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> userProductSupplierMappingRepository,
        IGenericRepository<PriceHunter.Model.Currency.Currency> currencyRepository,
        ICacheService cacheService,
        ILockService lockService,
        IMapper mapper,
        IValidationService validationService,
        ISendEndpointProvider sendEndpointProvider,
        IOptions<RabbitMqOption> rabbitMqOptions,
        ILogger<ProductService> logger)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _productSupplierInfoMappingRepository = productSupplierInfoMappingRepository;
            _productPriceHistoryRepository = productPriceHistoryRepository;
            _userProductSupplierMappingRepository = userProductSupplierMappingRepository;
            _currencyRepository = currencyRepository;
            _cacheService = cacheService;
            _lockService = lockService;
            _mapper = mapper;
            _validationService = validationService;
            _rabbitMqOptions = rabbitMqOptions.Value;
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductViewModel>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var productCacheKey = string.Format(CacheKeyConstants.ProductCacheKey, id);

            var product = await _cacheService.GetOrSetObjectAsync(productCacheKey, async () => await _productRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false, cancellationToken), CacheConstants.DefaultCacheDuration, cancellationToken);

            if (product == null)
            {
                return new ServiceResult<ProductViewModel>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Product)
                };
            }

            var productSupplierInfoMappingCacheKey = string.Format(CacheKeyConstants.ProductSupplierInfoMappingCacheKey, id);

            var productMappings = await _cacheService.GetOrSetObjectAsync(productCacheKey, () =>
                Task.FromResult(_productSupplierInfoMappingRepository.Find(p =>
                p.ProductId == product.Id &&
                p.IsDeleted == false).ToList()),
                CacheConstants.DefaultCacheDuration,
                cancellationToken
            );

            var productUserViewModel = _mapper.Map<ProductViewModel>(product);

            if (productMappings != null && productMappings.Any())
            {
                foreach (var mapping in productMappings)
                {
                    productUserViewModel.UrlSupplierMapping.Add(_mapper.Map<ProductSupplierInfoMappingViewModel>(mapping));
                }
            }

            return new ServiceResult<ProductViewModel>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = productUserViewModel
            };
        }

        public async Task<ServiceResult<ExpandoObject>> CreateAsync(CreateProductRequestServiceRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = _validationService.Validate(typeof(CreateProductRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            if (!await _currencyRepository.AnyAsync(p => p.Id == request.CurrencyId, cancellationToken))
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Currency)
                };
            }

            var productSupplierMappings = new List<ProductSupplierInfoMapping>();
            if (request.UrlSupplierMapping != null && request.UrlSupplierMapping.Any())
            {
                var errors = new List<string>();
                request.UrlSupplierMapping.ForEach(p => p.Url = p.Url.Trim());

                foreach (var mapping in request.UrlSupplierMapping)
                {
                    if (await _productSupplierInfoMappingRepository
                        .AnyAsync(p =>
                        p.Url.Equals(mapping.Url) &&
                        p.IsDeleted == false, cancellationToken))
                    {
                        errors.Add(string.Format(ServiceResponseMessage.PRODUCT_URL_DUPLICATE_ERROR, mapping.Url));
                    }

                    if (!await _supplierRepository.AnyAsync(p => p.Id == mapping.SupplierId && p.IsDeleted == false, cancellationToken))
                    {
                        errors.Add(String.Format(ServiceResponseMessage.SUPPLIER_NOTFOUND, mapping.SupplierId));
                    }
                }

                if (errors.Any())
                {
                    return new ServiceResult<ExpandoObject>
                    {
                        Status = ResultStatus.InvalidInput,
                        Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                        ValidationMessages = errors
                    };
                }

                foreach (var mapping in request.UrlSupplierMapping)
                {
                    productSupplierMappings.Add(new ProductSupplierInfoMapping
                    {
                        Url = mapping.Url,
                        SupplierId = mapping.SupplierId
                    });
                }
            }

            var entity = new PriceHunter.Model.Product.Product
            {
                Name = request.Name.Trim(),
                CurrencyId = request.CurrencyId
            };

            entity = await _productRepository.InsertAsync(entity, cancellationToken);

            productSupplierMappings.ForEach(p => p.ProductId = entity.Id);

            await _productSupplierInfoMappingRepository.InsertManyAsync(productSupplierMappings, cancellationToken);

            dynamic productWrapper = new ExpandoObject();
            productWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Created(Entities.Product, entity.Name),
                Data = productWrapper
            };
        }

        public async Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateProductRequestServiceRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = _validationService.Validate(typeof(UpdateProductRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            if (!await _currencyRepository.AnyAsync(p => p.Id == request.CurrencyId, cancellationToken))
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Currency)
                };
            }

            var entity = await _productRepository.FindOneAsync(p => p.Id == request.Id && p.IsDeleted == false, cancellationToken);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Product)
                };
            }

            var productSupplierMappings = new List<ProductSupplierInfoMapping>();
            if (request.UrlSupplierMapping != null && request.UrlSupplierMapping.Any())
            {
                var errors = new List<string>();
                request.UrlSupplierMapping.ForEach(p => p.Url = p.Url.Trim());

                foreach (var mapping in request.UrlSupplierMapping)
                {
                    if (await _productSupplierInfoMappingRepository
                        .AnyAsync(p =>
                        p.Url.Equals(mapping.Url) &&
                        p.IsDeleted == false, cancellationToken))
                    {
                        errors.Add(string.Format(ServiceResponseMessage.PRODUCT_URL_DUPLICATE_ERROR, mapping.Url));
                    }

                    if (!await _supplierRepository.AnyAsync(p => p.Id == mapping.SupplierId && p.IsDeleted == false, cancellationToken))
                    {
                        errors.Add(String.Format(ServiceResponseMessage.SUPPLIER_NOTFOUND, mapping.SupplierId));
                    }
                }

                if (errors.Any())
                {
                    return new ServiceResult<ExpandoObject>
                    {
                        Status = ResultStatus.InvalidInput,
                        Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                        ValidationMessages = errors
                    };
                }

                foreach (var mapping in request.UrlSupplierMapping)
                {
                    productSupplierMappings.Add(new ProductSupplierInfoMapping
                    {
                        Url = mapping.Url,
                        SupplierId = mapping.SupplierId
                    });
                }
            }

            var lockKey = string.Format(LockKeyConstants.ProductLockKey, entity.Id);
            var productCacheKey = string.Format(CacheKeyConstants.ProductCacheKey, entity.Id);
            var productSupplierInfoMappingCacheKey = string.Format(CacheKeyConstants.ProductSupplierInfoMappingCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey, cancellationToken))
            {
                entity.Name = request.Name.Trim();
                entity.CurrencyId = request.CurrencyId;

                entity = await _productRepository.UpdateAsync(entity, cancellationToken);

                var dbProductSupplierMappings = _productSupplierInfoMappingRepository.Find(p => p.ProductId == entity.Id && p.IsDeleted == false).ToList();

                if (dbProductSupplierMappings != null && dbProductSupplierMappings.Any())
                {
                    await _productSupplierInfoMappingRepository.DeleteManyAsync(dbProductSupplierMappings, cancellationToken);
                }

                if (productSupplierMappings.Any())
                {
                    await _productSupplierInfoMappingRepository.InsertManyAsync(productSupplierMappings, cancellationToken);
                }

                await _cacheService.RemoveAsync(productCacheKey, cancellationToken);
                await _cacheService.RemoveAsync(productSupplierInfoMappingCacheKey, cancellationToken);
            }

            dynamic productWrapper = new ExpandoObject();
            productWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Updated(Entities.Product, entity.Name),
                Data = productWrapper
            };
        }

        public async Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false, cancellationToken);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Product)
                };
            }

            if (await _userProductSupplierMappingRepository.AnyAsync(p => p.ProductId == entity.Id && p.IsDeleted == false, cancellationToken))
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = ServiceResponseMessage.PRODUCT_HAS_USER_MAPPING_CANNOT_DELETE
                };
            }

            var lockKey = string.Format(LockKeyConstants.ProductLockKey, entity.Id);
            var productCacheKey = string.Format(CacheKeyConstants.ProductCacheKey, entity.Id);
            var productSupplierInfoMappingCacheKey = string.Format(CacheKeyConstants.ProductSupplierInfoMappingCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey, cancellationToken))
            {
                await _productRepository.DeleteAsync(entity, cancellationToken);

                var dbProductSupplierMappings = _productSupplierInfoMappingRepository.Find(p => p.ProductId == entity.Id && p.IsDeleted == false).ToList();

                if (dbProductSupplierMappings != null && dbProductSupplierMappings.Any())
                {
                    await _productSupplierInfoMappingRepository.DeleteManyAsync(dbProductSupplierMappings, cancellationToken);
                }

                await _cacheService.RemoveAsync(productCacheKey, cancellationToken);
                await _cacheService.RemoveAsync(productSupplierInfoMappingCacheKey, cancellationToken);
            }

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Deleted(Entities.Product, entity.Name)
            };
        }

        public async Task<ServiceResult<PagedList<ProductSearchViewModel>>> SearchAsync(FilteryRequest request, CancellationToken cancellationToken)
        {
            var filteryResponse = await _productRepository
                .Find(p => p.IsDeleted == false)
                .BuildFilteryAsync(new ProductFilteryMapping(), request);

            var response = new PagedList<ProductSearchViewModel>
            {
                Data = _mapper.Map<List<ProductSearchViewModel>>(filteryResponse.Data),
                PageInfo = new Page
                {
                    PageNumber = filteryResponse.PageNumber,
                    PageSize = filteryResponse.PageSize,
                    TotalItemCount = filteryResponse.TotalItemCount
                }
            };

            return new ServiceResult<PagedList<ProductSearchViewModel>>
            {
                Data = response,
                Status = ResultStatus.Successful
            };
        }

        public async Task<ServiceResult<PagedList<ProductPriceHistorySearchViewModel>>> SearchPriceHistoryAsync(FilteryRequest request, CancellationToken cancellationToken)
        {
            var filteryResponse = await _productPriceHistoryRepository.Find(p => p.IsDeleted == false).BuildFilteryAsync(new ProductPriceHistoryFilteryMapping(), request);

            var response = new PagedList<ProductPriceHistorySearchViewModel>
            {
                Data = _mapper.Map<List<ProductPriceHistorySearchViewModel>>(filteryResponse.Data),
                PageInfo = new Page
                {
                    PageNumber = filteryResponse.PageNumber,
                    PageSize = filteryResponse.PageSize,
                    TotalItemCount = filteryResponse.TotalItemCount
                }
            };

            return new ServiceResult<PagedList<ProductPriceHistorySearchViewModel>>
            {
                Data = response,
                Status = ResultStatus.Successful
            };
        }

        [DisableConcurrentExecution(timeoutInSeconds: 60 * 60)]//60 * 60 = 1 hour
        public async Task CheckProductPricesAsync(Guid supplierId, CancellationToken cancellationToken)
        {
            try
            {
                var products = _productSupplierInfoMappingRepository.Find(p => p.SupplierId == supplierId && p.IsDeleted == false).ToList();
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"{_rabbitMqOptions.RabbitMqUri}/{_rabbitMqOptions.ParserQueue}"));

                if (products != null && products.Any())
                {
                    var supplier = await _supplierRepository.FindOneAsync(p => p.Id == supplierId && p.IsDeleted == false, cancellationToken);

                    foreach (var product in products)
                    {
                        await endpoint.Send(new SendParserCommand
                        {
                            ProductId = product.ProductId,
                            SupplierId = supplierId,
                            EnumMapping = supplier.EnumMapping,
                            Url = product.Url,
                            RequestTime = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task<ServiceResult<List<ProductPriceChangesViewModel>>> GetLastNMonthChangesAsync(Guid id, int monthCount, CancellationToken cancellationToken)
        {
            if (!await _productRepository.AnyAsync(p => p.Id == id && p.IsDeleted == false, cancellationToken))
            {
                return new ServiceResult<List<ProductPriceChangesViewModel>>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Product)
                };
            }

            var filterDate = DateTime.UtcNow;
            var filterDates = new Dictionary<int, List<int>>();
            for (int i = 1; i <= monthCount; i++)
            {
                if (filterDates.ContainsKey(filterDate.Year))
                {
                    if (!filterDates[filterDate.Year].Contains(filterDate.Month))
                    {
                        filterDates[filterDate.Year].Add(filterDate.Month);
                    }
                }
                else
                {
                    var months = new List<int>();
                    months.Add(filterDate.Month);
                    filterDates.Add(filterDate.Year, months);
                }

                filterDate = filterDate.AddMonths(-1);
            }

            var response = new List<ProductPriceChangesViewModel>();

            var groupedPriceHistory = new List<ProductPriceChangesViewModel>();
            foreach (var filter in filterDates)
            {
                foreach (var filterMonth in filter.Value)
                {
                    var minPrice = _productPriceHistoryRepository
                                   .Find(p =>
                                       p.ProductId == id &&
                                       p.Year == filter.Key &&
                                       p.Month == filterMonth &&
                                       p.Price != 0 &&
                                       p.IsDeleted == false)
                                   .OrderBy(p => p.Price)
                                   .FirstOrDefault();

                    if (minPrice == null)
                    {
                        groupedPriceHistory.Add(new ProductPriceChangesViewModel() { Year = filter.Key, Month = filterMonth, Price = 0 });
                    }
                    else
                    {
                        groupedPriceHistory.Add(new ProductPriceChangesViewModel() { Year = filter.Key, Month = filterMonth, Price = minPrice.Price, SupplierId = minPrice.SupplierId });
                    }
                }
            }

            if (groupedPriceHistory != null && groupedPriceHistory.Any())
            {
                response = groupedPriceHistory
                .OrderByDescending(p => p.Year)
                .OrderByDescending(p => p.Month)
                .ToList();
            }

            return new ServiceResult<List<ProductPriceChangesViewModel>>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = response
            };
        }
    }
}
