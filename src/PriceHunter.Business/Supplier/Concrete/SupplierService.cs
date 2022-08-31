using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PriceHunter.Business.Supplier.Abstract;
using PriceHunter.Cache.Abstract;
using PriceHunter.Cache.Constants;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract; 
using PriceHunter.Contract.App.Supplier;
using PriceHunter.Resources.Extensions;

namespace PriceHunter.Business.Supplier.Concrete
{
    public class SupplierService : ISupplierService
    {
        IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;
        private readonly ICacheService _cacheService; 
        private readonly IMapper _mapper;

        public SupplierService(
            IGenericRepository<PriceHunter.Model.Supplier.Supplier> supplierRepository,
            ICacheService cacheService, 
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _cacheService = cacheService; 
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<SupplierViewModel>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var suppliers = await _cacheService.GetOrSetObjectAsync(CacheKeyConstants.SuppliersAllCacheKey, () => _supplierRepository.Find(p => p.IsDeleted == false).ToList(), CacheConstants.DefaultCacheDuration, cancellationToken);

            var response = new List<SupplierViewModel>();

            foreach (var supplier in suppliers)
            {
                response.Add(_mapper.Map<SupplierViewModel>(supplier));
            }

            return new ServiceResult<List<SupplierViewModel>>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = response
            };
        }
    }
}
