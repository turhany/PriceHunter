using AutoMapper;
using PriceHunter.Business.User.Abstract;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Cache.Abstract;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Contract.App.User;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model; 

namespace PriceHunter.Business.User.Concrete
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<PriceHunter.Model.User.User> _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public UserService(
            IGenericRepository<Model.User.User> userRepository, 
            ICacheService cacheService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<UserViewModel>> GetAsync(Guid id)
        {
            var cacheKey = string.Format(CacheKeyConstants.UserCacheKey, id);

            var user = await _cacheService.GetOrSetObjectAsync(cacheKey, async () => await _userRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false));

            if (user == null)
            {
                return new ServiceResult<UserViewModel>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.User)
                };
            }

            return new ServiceResult<UserViewModel>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = _mapper.Map<UserViewModel>(user)
            };
        }
    }
}
