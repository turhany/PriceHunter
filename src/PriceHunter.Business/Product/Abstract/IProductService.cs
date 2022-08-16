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
        Task<ServiceResult<ProductViewModel>> GetAsync(Guid id);
        Task<ServiceResult<PagedList<ProductSearchViewModel>>> SearchAsync(FilteryRequest request);
        Task<ServiceResult<PagedList<ProductPriceHistorySearchViewModel>>> SearchPriceHistoryAsync(FilteryRequest request);        
        Task<ServiceResult<ExpandoObject>> CreateAsync(CreateProductRequestServiceRequest request);
        Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateProductRequestServiceRequest request);
        Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id);
    }
}
