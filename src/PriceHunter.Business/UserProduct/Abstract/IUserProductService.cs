using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Service.UserProduct;
using System.Dynamic;

namespace PriceHunter.Business.UserProduct.Abstract
{
    public interface IUserProductService : IService
    {
        Task<ServiceResult<UserProductViewModel>> GetAsync(Guid id);
        Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserProductRequestServiceRequest request);
        Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserProductRequestServiceRequest request);
        Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id);
    }
}
