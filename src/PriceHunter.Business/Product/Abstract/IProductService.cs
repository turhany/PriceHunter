using Filtery.Models;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Pager;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Service.Product;
using System.Dynamic;

namespace PriceHunter.Business.Product.Abstract
{
    public interface IProductService : IService
    {
        Task<ServiceResult<ProductViewModel>> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<ServiceResult<PagedList<ProductSearchViewModel>>> SearchAsync(FilteryRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<PagedList<ProductPriceHistorySearchViewModel>>> SearchPriceHistoryAsync(FilteryRequest request, CancellationToken cancellationToken);        
        Task<ServiceResult<ExpandoObject>> CreateAsync(CreateProductRequestServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateProductRequestServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<ServiceResult<List<ProductPriceChangesViewModel>>> GetLastNMonthChangesAsync(Guid id, int monthCount, CancellationToken cancellationToken);        
    }
}
