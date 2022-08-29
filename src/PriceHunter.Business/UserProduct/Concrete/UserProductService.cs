using AutoMapper;
using Filtery.Extensions;
using Filtery.Models;
using PriceHunter.Business.UserProduct.Abstract;
using PriceHunter.Business.UserProduct.Validator;
using PriceHunter.Cache.Abstract;
using PriceHunter.Common.Application;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Pager;
using PriceHunter.Common.Validation.Abstract;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Mappings.Filtery;
using PriceHunter.Contract.Service.UserProduct; 
using PriceHunter.Model.UserProduct;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model;
using PriceHunter.Resources.Service;
using System.Dynamic;
using PriceHunter.Lock.Abstract;
using PriceHunter.Cache.Constants;

namespace PriceHunter.Business.UserProduct.Concrete
{
    public class UserProductService : IUserProductService
    {
        private readonly IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.Product> _productRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> _productSupplierInfoMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProduct> _userProductRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> _userProductSupplierMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.ProductPriceHistory> _productPriceHistoryRepository;
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public UserProductService(
        IGenericRepository<PriceHunter.Model.Supplier.Supplier> supplierRepository,
        IGenericRepository<PriceHunter.Model.Product.Product> productRepository,
        IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> productSupplierInfoMappingRepository,
        IGenericRepository<PriceHunter.Model.UserProduct.UserProduct> userProductRepository,
        IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> userProductSupplierMappingRepository,
        IGenericRepository<PriceHunter.Model.Product.ProductPriceHistory> productPriceHistoryRepository,
        ICacheService cacheService,
        ILockService lockService,
        IMapper mapper,
        IValidationService validationService)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _productSupplierInfoMappingRepository = productSupplierInfoMappingRepository;
            _userProductRepository = userProductRepository;
            _userProductSupplierMappingRepository = userProductSupplierMappingRepository;
            _productPriceHistoryRepository = productPriceHistoryRepository;
            _cacheService = cacheService;
            _lockService = lockService;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<ServiceResult<UserProductViewModel>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var cacheKey = string.Format(CacheKeyConstants.UserProductCacheKey, id);
            var userId = ApplicationContext.Instance.CurrentUser.Id;

            var product = await _cacheService.GetOrSetObjectAsync(
                cacheKey,
                async () => await _userProductRepository.FindOneAsync(p => p.Id == id && p.UserId == userId && p.IsDeleted == false, cancellationToken),
                CacheConstants.DefaultCacheDuration,
                cancellationToken);

            if (product == null || product.UserId != userId)
            {
                return new ServiceResult<UserProductViewModel>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.UserProduct)
                };
            }

            var productViewModel = _mapper.Map<UserProductViewModel>(product);


            var userProductMappings = _userProductSupplierMappingRepository.Find(p =>
            p.UserProductId == product.Id &&
            p.IsDeleted == false).ToList();

            if (userProductMappings != null && userProductMappings.Any())
            {
                foreach (var mapping in userProductMappings)
                {
                    productViewModel.UrlSupplierMapping.Add(_mapper.Map<UrlSupplierMappingViewModel>(mapping));
                }
            }

            return new ServiceResult<UserProductViewModel>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = productViewModel
            };
        }

        public async Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserProductRequestServiceRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = _validationService.Validate(typeof(CreateUserProductRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            var userId = ApplicationContext.Instance.CurrentUser.Id;
            var productSupplierMappings = new List<UserProductSupplierMapping>();
            if (request.UrlSupplierMapping != null && request.UrlSupplierMapping.Any())
            {
                var errors = new List<string>();
                request.UrlSupplierMapping.ForEach(p => p.Url = p.Url.Trim());

                foreach (var mapping in request.UrlSupplierMapping)
                {
                    if (request.UrlSupplierMapping.Select(p => p.Url).GroupBy(p => p).Any(p => p.Count() > 1))
                    {
                        var errorMessage = string.Format(ServiceResponseMessage.USERPRODUCT_URL_DUPLICATE_ERROR, mapping.Url);
                        if (!errors.Contains(errorMessage))
                        {
                            errors.Add(errorMessage);
                        }
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


                Guid createdProductId = Guid.Empty;
                foreach (var mapping in request.UrlSupplierMapping)
                {
                    Guid productId = Guid.Empty;
                    var productSupplierInfoMapping = await _productSupplierInfoMappingRepository.FindOneAsync(p =>
                        p.Url.Equals(mapping.Url) &&
                        p.IsDeleted == false, cancellationToken);

                    if (productSupplierInfoMapping == null)
                    {
                        if (createdProductId == Guid.Empty)
                        {
                            var product = await _productRepository.InsertAsync(new Model.Product.Product
                            {
                                Name = request.Name
                            }, cancellationToken);
                            createdProductId = product.Id;
                        }

                        await _productSupplierInfoMappingRepository.InsertAsync(new Model.Product.ProductSupplierInfoMapping
                        {
                            ProductId = createdProductId,
                            SupplierId = mapping.SupplierId,
                            Url = mapping.Url
                        }, cancellationToken);
                    }
                    else
                    {
                        productId = productSupplierInfoMapping.ProductId;
                    }

                    var productIdForMap = productId == Guid.Empty ? createdProductId : productId;
                    productSupplierMappings.Add(new UserProductSupplierMapping
                    {
                        Url = mapping.Url,
                        ProductId = productIdForMap,
                        SupplierId = mapping.SupplierId
                    });
                }
            }

            var entity = new PriceHunter.Model.UserProduct.UserProduct
            {
                Name = request.Name.Trim(),
                UserId = userId
            };

            entity = await _userProductRepository.InsertAsync(entity, cancellationToken);

            if (productSupplierMappings.Any())
            {
                productSupplierMappings.ForEach(p => p.UserProductId = entity.Id);
                await _userProductSupplierMappingRepository.InsertManyAsync(productSupplierMappings, cancellationToken);
            }

            dynamic productWrapper = new ExpandoObject();
            productWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Created(Entities.Product, entity.Name),
                Data = productWrapper
            };
        }

        public async Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserProductRequestServiceRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = _validationService.Validate(typeof(UpdateUserProductRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }
            var userId = ApplicationContext.Instance.CurrentUser.Id;

            var entity = await _userProductRepository.FindOneAsync(p =>
               p.Id == request.Id &&
               p.UserId == userId &&
               p.IsDeleted == false, cancellationToken);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.UserProduct)
                };
            }

            var productSupplierMappings = new List<UserProductSupplierMapping>();
            if (request.UrlSupplierMapping != null && request.UrlSupplierMapping.Any())
            {
                var errors = new List<string>();
                request.UrlSupplierMapping.ForEach(p => p.Url = p.Url.Trim());

                foreach (var mapping in request.UrlSupplierMapping)
                {
                    if (request.UrlSupplierMapping.Select(p => p.Url).GroupBy(p => p).Any(p => p.Count() > 1))
                    {
                        var errorMessage = string.Format(ServiceResponseMessage.USERPRODUCT_URL_DUPLICATE_ERROR, mapping.Url);
                        if (!errors.Contains(errorMessage))
                        {
                            errors.Add(errorMessage);
                        }                        
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

                Guid createdProductId = Guid.Empty;
                foreach (var mapping in request.UrlSupplierMapping)
                {
                    Console.WriteLine(mapping.SupplierId);
                    Console.WriteLine(mapping.Url);

                    Guid productId = Guid.Empty;
                    var productSupplierInfoMapping = await _productSupplierInfoMappingRepository.FindOneAsync(p =>
                       p.Url.Equals(mapping.Url) &&
                       p.IsDeleted == false, cancellationToken);

                    if (productSupplierInfoMapping == null)
                    {
                        if (createdProductId == Guid.Empty)
                        {
                            var product = await _productRepository.InsertAsync(new Model.Product.Product
                            {
                                Name = request.Name
                            }, cancellationToken);
                            createdProductId = product.Id;
                        }

                        await _productSupplierInfoMappingRepository.InsertAsync(new Model.Product.ProductSupplierInfoMapping
                        {
                            ProductId = createdProductId,
                            SupplierId = mapping.SupplierId,
                            Url = mapping.Url
                        }, cancellationToken);
                    }
                    else
                    {
                        productId = productSupplierInfoMapping.ProductId;
                    }

                    var productIdForMap = productId == Guid.Empty ? createdProductId : productId;
                    productSupplierMappings.Add(new UserProductSupplierMapping
                    {
                        Url = mapping.Url,
                        ProductId = productIdForMap,
                        SupplierId = mapping.SupplierId,
                        UserProductId = request.Id                       
                    });
                }
            }

            var lockKey = string.Format(LockKeyConstants.UserProductLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.UserProductCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey, cancellationToken))
            {
                entity.Name = request.Name.Trim();

                entity = await _userProductRepository.UpdateAsync(entity, cancellationToken);

                var dbProductSupplierMappings = _userProductSupplierMappingRepository.Find(p =>
                    p.UserProductId == request.Id && 
                    p.IsDeleted == false).ToList();

                if (dbProductSupplierMappings.Any())
                {
                    await _userProductSupplierMappingRepository.DeleteManyAsync(dbProductSupplierMappings, cancellationToken);
                }

                if (productSupplierMappings.Any())
                {
                    productSupplierMappings.ForEach(p => p.UserProductId = entity.Id);
                    await _userProductSupplierMappingRepository.InsertManyAsync(productSupplierMappings, cancellationToken);
                }

                await _cacheService.RemoveAsync(cacheKey, cancellationToken);
            }


            dynamic productWrapper = new ExpandoObject();
            productWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Updated(Entities.UserProduct, entity.Name),
                Data = productWrapper
            };
        }

        public async Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var userId = ApplicationContext.Instance.CurrentUser.Id;

            var entity = await _userProductRepository.FindOneAsync(p =>
                p.Id == id &&
                p.UserId == userId &&
                p.IsDeleted == false, cancellationToken);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.UserProduct)
                };
            }

            var lockKey = string.Format(LockKeyConstants.UserProductLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.UserProductCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey, cancellationToken))
            {
                await _userProductRepository.DeleteAsync(entity, cancellationToken);

                var dbProductSupplierMappings = _userProductSupplierMappingRepository.Find(p =>
                    p.UserProductId == entity.Id &&
                    p.IsDeleted == false).ToList();

                if (dbProductSupplierMappings.Any())
                {
                    await _userProductSupplierMappingRepository.DeleteManyAsync(dbProductSupplierMappings, cancellationToken);
                }

                await _cacheService.RemoveAsync(cacheKey, cancellationToken);
            }

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Deleted(Entities.UserProduct, entity.Name)
            };
        }

        public async Task<ServiceResult<PagedList<UserProductSearchViewModel>>> SearchAsync(FilteryRequest request, CancellationToken cancellationToken)
        {
            var filteryResponse = await _userProductRepository
                .Find(p => p.IsDeleted == false)
                .BuildFilteryAsync(new UserProductFilteryMapping(), request);

            var response = new PagedList<UserProductSearchViewModel>
            {
                Data = _mapper.Map<List<UserProductSearchViewModel>>(filteryResponse.Data),
                PageInfo = new Page
                {
                    PageNumber = filteryResponse.PageNumber,
                    PageSize = filteryResponse.PageSize,
                    TotalItemCount = filteryResponse.TotalItemCount
                }
            };

            return new ServiceResult<PagedList<UserProductSearchViewModel>>
            {
                Data = response,
                Status = ResultStatus.Successful
            };
        }

        public async Task<ServiceResult<List<ProductPriceChangesViewModel>>> GetLastNMonthChangesAsync(Guid id, int monthCount, CancellationToken cancellationToken)
        {
            var userId = ApplicationContext.Instance.CurrentUser.Id;
            var userProduct = await _userProductRepository.FindOneAsync(p => p.Id == id && p.UserId == userId && p.IsDeleted == false, cancellationToken);

            if (userProduct == null)
            {
                return new ServiceResult<List<ProductPriceChangesViewModel>>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.UserProduct)
                };
            }

            var response = new List<ProductPriceChangesViewModel>();

            var userProductMappings = _userProductSupplierMappingRepository.Find(p =>
               p.UserProductId == userProduct.Id && 
               p.IsDeleted == false).ToList();

            if (userProductMappings == null)
            {
                return new ServiceResult<List<ProductPriceChangesViewModel>>
                {
                    Status = ResultStatus.Successful,
                    Message = Resource.Retrieved(),
                    Data = response
                };
            }

            List<Guid> productIds = userProductMappings.Select(p => p.ProductId.Value).ToList();
            if (!await _productRepository.AnyAsync(p => productIds.Contains(p.Id) && p.IsDeleted == false, cancellationToken))
            {
                return new ServiceResult<List<ProductPriceChangesViewModel>>
                {
                    Status = ResultStatus.Successful,
                    Message = Resource.Retrieved(),
                    Data = response
                };
            }

            var groupedPriceHistory = _productPriceHistoryRepository
                                    .Find(p => productIds.Contains(p.ProductId) && p.IsDeleted == false)
                                    .OrderByDescending(p => p.CreatedBy)
                                    .GroupBy(p => new { p.Year, p.Month })
                                    .Select(p => new { Year = p.Key.Year, Month = p.Key.Month, Price = p.Average(x => x.Price) })
                                    .Take(monthCount)
                                    .ToList();



            if (groupedPriceHistory != null && groupedPriceHistory.Any())
            {
                response = groupedPriceHistory.Select(p => new ProductPriceChangesViewModel
                {
                    Year = p.Year,
                    Month = p.Month,
                    Price = p.Price
                }).ToList();
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
