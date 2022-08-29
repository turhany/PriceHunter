using Filtery.Models;
using PriceHunter.Common.Auth.Concrete;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Pager;
using PriceHunter.Contract.App.User;
using PriceHunter.Contract.Service.User;
using System.Dynamic;

namespace PriceHunter.Business.User.Abstract
{
    public interface IUserService : IService
    {
        Task<ServiceResult<UserViewModel>> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<ServiceResult<PagedList<UserViewModel>>> SearchAsync(FilteryRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserRequestServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserRequestServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<UserProfileImageViewModel>> UploadProfileImageAsync(ProfileFileContractServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id, CancellationToken cancellationToken);



        Task<ServiceResult<AccessTokenContract>> GetTokenAsync(GetTokenContractServiceRequest request, CancellationToken cancellationToken);
        Task<ServiceResult<AccessTokenContract>> RefreshTokenAsync(RefreshTokenContractServiceRequest request, CancellationToken cancellationToken);
    }
}
