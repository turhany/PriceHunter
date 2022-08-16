using AutoMapper;
using PriceHunter.Business.UserProduct.Abstract;
using PriceHunter.Business.UserProduct.Validator;
using PriceHunter.Common.Application;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Cache.Abstract;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Lock.Abstract;
using PriceHunter.Common.Validation.Abstract;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Service.UserProduct;
using PriceHunter.Model.UserProduct;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model;
using PriceHunter.Resources.Service;
using System.Dynamic;

namespace PriceHunter.Business.UserProduct.Concrete
{
    public class UserProductService : IUserProductService
    {
        private readonly IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.Product> _productRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> _productSupplierInfoMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProduct> _userProductRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> _userProductSupplierMappingRepository;
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
            _cacheService = cacheService;
            _lockService = lockService;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<ServiceResult<UserProductViewModel>> GetAsync(Guid id)
        {
            var cacheKey = string.Format(CacheKeyConstants.UserProductCacheKey, id);
            var userId = ApplicationContext.Instance.CurrentUser.Id;

            var product = await _cacheService.GetOrSetObjectAsync(
                cacheKey,
                async () => await _userProductRepository.FindOneAsync(p => p.Id == id && p.UserId == userId && p.IsDeleted == false));

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
            p.UserId == userId &&
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

        public async Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserProductRequestServiceRequest request)
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
                    if (await _userProductSupplierMappingRepository
                        .AnyAsync(p =>
                        p.Url.Equals(mapping.Url) &&
                        p.IsDeleted == false &&
                        p.UserId == userId))
                    {
                        errors.Add(string.Format(ServiceResponseMessage.USERPRODUCT_URL_DUPLICATE_ERROR, mapping.Url));
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


                Guid createdProductId = Guid.Empty;
                foreach (var mapping in request.UrlSupplierMapping)
                {
                    Guid productId = Guid.Empty;
                    var productSupplierInfoMapping = await _productSupplierInfoMappingRepository.FindOneAsync(p =>
                        p.Url.Equals(mapping.Url) &&
                        p.IsDeleted == false);

                    if (productSupplierInfoMapping == null)
                    {
                        if (createdProductId == Guid.Empty)
                        {
                            var product = await _productRepository.InsertAsync(new Model.Product.Product
                            {
                                Name = request.Name
                            });
                            createdProductId = product.Id;
                        }

                        await _productSupplierInfoMappingRepository.InsertAsync(new Model.Product.ProductSupplierInfoMapping
                        {
                            ProductId = createdProductId,
                            SupplierId = mapping.SupplierId,
                            Url = mapping.Url
                        });
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
                        UserId = userId
                    });
                }
            }

            var entity = new PriceHunter.Model.UserProduct.UserProduct
            {
                Name = request.Name.Trim(),
                UserId = userId
            };

            entity = await _userProductRepository.InsertAsync(entity);

            if (productSupplierMappings.Any())
            {
                productSupplierMappings.ForEach(p => p.UserProductId = entity.Id);
                await _userProductSupplierMappingRepository.InsertManyAsync(productSupplierMappings);
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

        public async Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserProductRequestServiceRequest request)
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
               p.IsDeleted == false);

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
                    if (await _userProductSupplierMappingRepository
                        .AnyAsync(p =>
                        p.Url.Equals(mapping.Url) &&
                        p.IsDeleted == false &&
                        p.UserId == userId))
                    {
                        errors.Add(string.Format(ServiceResponseMessage.USERPRODUCT_URL_DUPLICATE_ERROR, mapping.Url));
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

                Guid createdProductId = Guid.Empty;
                foreach (var mapping in request.UrlSupplierMapping)
                {
                    Guid productId = Guid.Empty;
                    var productSupplierInfoMapping = await _productSupplierInfoMappingRepository.FindOneAsync(p =>
                       p.Url.Equals(mapping.Url) &&
                       p.IsDeleted == false);

                    if (productSupplierInfoMapping == null)
                    {
                        if (createdProductId == Guid.Empty)
                        {
                            var product = await _productRepository.InsertAsync(new Model.Product.Product
                            {
                                Name = request.Name
                            });
                            createdProductId = product.Id;
                        }

                        await _productSupplierInfoMappingRepository.InsertAsync(new Model.Product.ProductSupplierInfoMapping
                        {
                            ProductId = createdProductId,
                            SupplierId = mapping.SupplierId,
                            Url = mapping.Url
                        });
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
                        UserId = userId
                    });
                }
            }

            var lockKey = string.Format(LockKeyConstants.UserProductLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.UserProductCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey))
            {
                entity.Name = request.Name.Trim();

                entity = await _userProductRepository.UpdateAsync(entity);

                var dbProductSupplierMappings = _userProductSupplierMappingRepository.Find(p =>
                    p.IsDeleted == false &&
                    p.UserId == userId).ToList();

                if (dbProductSupplierMappings.Any())
                {
                    await _userProductSupplierMappingRepository.DeleteManyAsync(dbProductSupplierMappings);
                }

                if (productSupplierMappings.Any())
                {
                    productSupplierMappings.ForEach(p => p.UserProductId = entity.Id);
                    await _userProductSupplierMappingRepository.InsertManyAsync(productSupplierMappings);
                }

                await _cacheService.RemoveAsync(cacheKey);
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

        public async Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id)
        {
            var userId = ApplicationContext.Instance.CurrentUser.Id;

            var entity = await _userProductRepository.FindOneAsync(p =>
                p.Id == id &&
                p.UserId == userId &&
                p.IsDeleted == false);

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

            using (await _lockService.CreateLockAsync(lockKey))
            {
                await _userProductRepository.DeleteAsync(entity);

                var dbProductSupplierMappings = _userProductSupplierMappingRepository.Find(p =>
                    p.UserProductId == entity.Id &&
                    p.IsDeleted == false &&
                    p.UserId == userId).ToList();

                if (dbProductSupplierMappings.Any())
                {
                    await _userProductSupplierMappingRepository.DeleteManyAsync(dbProductSupplierMappings);
                }

                await _cacheService.RemoveAsync(cacheKey);
            }

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Deleted(Entities.UserProduct, entity.Name)
            };
        }
    }
}
