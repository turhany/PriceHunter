using AutoMapper;
using Filtery.Extensions;
using Filtery.Models;
using MassTransit; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceHunter.Business.Product.Abstract;
using PriceHunter.Business.Product.Validator;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Cache.Abstract;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Lock.Abstract;
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

namespace PriceHunter.Business.Product.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.Product> _productRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> _productSupplierInfoMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> _userProductSupplierMappingRepository;
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
        IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> userProductSupplierMappingRepository,
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
            _userProductSupplierMappingRepository = userProductSupplierMappingRepository;
            _cacheService = cacheService;
            _lockService = lockService;
            _mapper = mapper;
            _validationService = validationService;
            _rabbitMqOptions = rabbitMqOptions.Value;
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductViewModel>> GetAsync(Guid id)
        {
            var productCacheKey = string.Format(CacheKeyConstants.ProductCacheKey, id);

            var product = await _cacheService.GetOrSetObjectAsync(productCacheKey, async () => await _productRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false));

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
                p.IsDeleted == false).ToList())
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

        public async Task<ServiceResult<ExpandoObject>> CreateAsync(CreateProductRequestServiceRequest request)
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
                        p.IsDeleted == false))
                    {
                        errors.Add(string.Format(ServiceResponseMessage.PRODUCT_URL_DUPLICATE_ERROR, mapping.Url));
                    }

                    if (!await _supplierRepository.AnyAsync(p => p.Id == mapping.SupplierId && p.IsDeleted == false))
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
                Name = request.Name.Trim()
            };

            entity = await _productRepository.InsertAsync(entity);

            productSupplierMappings.ForEach(p => p.ProductId = entity.Id);

            await _productSupplierInfoMappingRepository.InsertManyAsync(productSupplierMappings);

            dynamic productWrapper = new ExpandoObject();
            productWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Created(Entities.Product, entity.Name),
                Data = productWrapper
            };
        }

        public async Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateProductRequestServiceRequest request)
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

            var entity = await _productRepository.FindOneAsync(p => p.Id == request.Id && p.IsDeleted == false);

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
                        p.IsDeleted == false))
                    {
                        errors.Add(string.Format(ServiceResponseMessage.PRODUCT_URL_DUPLICATE_ERROR, mapping.Url));
                    }

                    if (!await _supplierRepository.AnyAsync(p => p.Id == mapping.SupplierId && p.IsDeleted == false))
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

            using (await _lockService.CreateLockAsync(lockKey))
            {
                entity.Name = request.Name.Trim();

                entity = await _productRepository.UpdateAsync(entity);

                var dbProductSupplierMappings = _productSupplierInfoMappingRepository.Find(p => p.ProductId == entity.Id && p.IsDeleted == false).ToList();

                if (dbProductSupplierMappings != null && dbProductSupplierMappings.Any())
                {
                    await _productSupplierInfoMappingRepository.DeleteManyAsync(dbProductSupplierMappings);
                }

                if (productSupplierMappings.Any())
                {
                    await _productSupplierInfoMappingRepository.InsertManyAsync(productSupplierMappings);
                }

                await _cacheService.RemoveAsync(productCacheKey);
                await _cacheService.RemoveAsync(productSupplierInfoMappingCacheKey);
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

        public async Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id)
        {
            var entity = await _productRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Product)
                };
            }

            if (await _userProductSupplierMappingRepository.AnyAsync(p => p.ProductId == entity.Id && p.IsDeleted == false))
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

            using (await _lockService.CreateLockAsync(lockKey))
            {
                await _productRepository.DeleteAsync(entity);

                var dbProductSupplierMappings = _productSupplierInfoMappingRepository.Find(p => p.ProductId == entity.Id && p.IsDeleted == false).ToList();

                if (dbProductSupplierMappings != null && dbProductSupplierMappings.Any())
                {
                    await _productSupplierInfoMappingRepository.DeleteManyAsync(dbProductSupplierMappings);
                }

                await _cacheService.RemoveAsync(productCacheKey);
                await _cacheService.RemoveAsync(productSupplierInfoMappingCacheKey);
            }

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Deleted(Entities.Product, entity.Name)
            };
        }

        public async Task<ServiceResult<PagedList<ProductSearchViewModel>>> SearchAsync(FilteryRequest request)
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

        public async Task CheckProductPricesAsync(Guid supplierId)
        {
            try
            {
                var products = _productSupplierInfoMappingRepository.Find(p => p.SupplierId == supplierId && p.IsDeleted == false).ToList();
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"{_rabbitMqOptions.RabbitMqUri}/{_rabbitMqOptions.ParserQueue}"));

                if (products != null && products.Any())
                {
                    var supplier = await _supplierRepository.FindOneAsync(p => p.Id == supplierId && p.IsDeleted == false);

                    foreach (var product in products)
                    {
                        await endpoint.Send(new SendParserCommand
                        {
                            ProductId = product.ProductId,
                            SupplierId = supplierId,
                            EnumMapping = supplier.EnumMapping,
                            Url = product.Url,
                            RequestTime = DateTime.UtcNow
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
