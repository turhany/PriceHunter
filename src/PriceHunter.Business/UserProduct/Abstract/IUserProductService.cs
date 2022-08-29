using Filtery.Models;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Pager;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Service.UserProduct;
using System.Dynamic;

namespace PriceHunter.Business.UserProduct.Abstract
{
    public interface IUserProductService : IService
    {
        Task<ServiceResult<UserProductViewModel>> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<ServiceResult<PagedList<UserProductSearchViewModel>>> SearchAsync(FilteryRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserProductRequestServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserProductRequestServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<ServiceResult<List<ProductPriceChangesViewModel>>> GetLastNMonthChangesAsync(Guid id, int monthCount, CancellationToken cancellationToken);
    }
}
