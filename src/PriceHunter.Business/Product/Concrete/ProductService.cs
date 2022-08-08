using AutoMapper;
using PriceHunter.Business.Product.Abstract;
using PriceHunter.Business.Product.Validator;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Cache.Abstract;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Lock.Abstract;
using PriceHunter.Common.Validation.Abstract;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Service.Product;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model;
using PriceHunter.Resources.Service;
using System.Dynamic;

namespace PriceHunter.Business.Product.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<PriceHunter.Model.Product.Product> _productRepository; 
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public ProductService(
        IGenericRepository<PriceHunter.Model.Product.Product> productRepository, 
        ICacheService cacheService,
        ILockService lockService,
        IMapper mapper,
        IValidationService validationService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _lockService = lockService;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<ServiceResult<ProductViewModel>> GetAsync(Guid id)
        {
            var cacheKey = string.Format(CacheKeyConstants.ProductCacheKey, id);

            var product = await _cacheService.GetOrSetObjectAsync(cacheKey, async () => await _productRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false));

            if (product == null)
            {
                return new ServiceResult<ProductViewModel>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.Product)
                };
            }

            return new ServiceResult<ProductViewModel>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = _mapper.Map<ProductViewModel>(product)
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
             
            var entity = new PriceHunter.Model.Product.Product
            {
                Name = request.Name.Trim()
            };

            entity = await _productRepository.InsertAsync(entity);

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

            var lockKey = string.Format(LockKeyConstants.ProductLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.ProductCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey))
            {
                entity.Name = request.Name.Trim();

                entity = await _productRepository.UpdateAsync(entity);

                await _cacheService.RemoveAsync(cacheKey);
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

            var lockKey = string.Format(LockKeyConstants.ProductLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.ProductCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey))
            {
                await _productRepository.DeleteAsync(entity);

                await _cacheService.RemoveAsync(cacheKey);
            }

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Deleted(Entities.Product, entity.Name)
            };
        }
    }
}
