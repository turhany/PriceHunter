using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.User;

namespace PriceHunter.Business.User.Abstract
{
    public interface IUserService : IService
    {
        Task<ServiceResult<UserViewModel>> GetAsync(Guid id);
    }
}
