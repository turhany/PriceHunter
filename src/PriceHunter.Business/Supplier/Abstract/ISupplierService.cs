using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.Supplier;

namespace PriceHunter.Business.Supplier.Abstract
{
    public interface ISupplierService : IService
    {
        Task<ServiceResult<List<SupplierViewModel>>> GetAllAsync(CancellationToken cancellationToken);
    }
}
