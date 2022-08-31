using AutoMapper;
using PriceHunter.Business.Currency.Abstract;
using PriceHunter.Cache.Abstract;
using PriceHunter.Cache.Constants;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.Currency;
using PriceHunter.Resources.Extensions; 

namespace PriceHunter.Business.Currency.Concrete
{
    public class CurrencyService : ICurrencyService
    {
        IGenericRepository<PriceHunter.Model.Currency.Currency> _currencyRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public CurrencyService(
            IGenericRepository<PriceHunter.Model.Currency.Currency> currencyRepository,
            ICacheService cacheService,
            IMapper mapper)
        {
            _currencyRepository = currencyRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<CurrencyViewModel>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var currencies = await _cacheService.GetOrSetObjectAsync(CacheKeyConstants.CurrenciesAllCacheKey, () => _currencyRepository.Find(p => p.IsDeleted == false).ToList(), CacheConstants.DefaultCacheDuration, cancellationToken);

            var response = new List<CurrencyViewModel>();

            foreach (var currency in currencies)
            {
                response.Add(_mapper.Map<CurrencyViewModel>(currency));
            }

            return new ServiceResult<List<CurrencyViewModel>>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = response
            };
        }
    }
}
