using PriceHunter.Common.Auth.Concrete;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.User;
using PriceHunter.Contract.Service.User;
using System.Dynamic;

namespace PriceHunter.Business.User.Abstract
{
    public interface IUserService : IService
    {
        Task<ServiceResult<UserViewModel>> GetAsync(Guid id);
        Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserRequestServiceRequest request);
        Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserRequestServiceRequest request);
        Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id);
         

        Task<ServiceResult<AccessTokenContract>> GetTokenAsync(GetTokenContractServiceRequest request);
        Task<ServiceResult<AccessTokenContract>> RefreshTokenAsync(RefreshTokenContractServiceRequest request);
    }
}
