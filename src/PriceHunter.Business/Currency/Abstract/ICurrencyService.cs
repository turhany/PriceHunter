using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.Currency; 

namespace PriceHunter.Business.Currency.Abstract
{
    public interface ICurrencyService : IService
    {
        Task<ServiceResult<List<CurrencyViewModel>>> GetAllAsync(CancellationToken cancellationToken);
    }
}
 