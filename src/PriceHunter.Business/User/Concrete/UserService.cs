using AutoMapper;
using Filtery.Extensions;
using Filtery.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceHunter.Business.User.Abstract;
using PriceHunter.Business.User.Validator;
using PriceHunter.Cache.Abstract;
using PriceHunter.Common.Application;
using PriceHunter.Common.Auth;
using PriceHunter.Common.Auth.Concrete;
using PriceHunter.Common.BaseModels.Service;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Common.Options;
using PriceHunter.Common.Pager;
using PriceHunter.Common.Validation.Abstract;
using PriceHunter.Contract.App.User;
using PriceHunter.Contract.Mappings.Filtery;
using PriceHunter.Contract.Service.User;
using PriceHunter.Resources.Extensions;
using PriceHunter.Resources.Model;
using PriceHunter.Resources.Service;
using System.Dynamic;
using PriceHunter.Lock.Abstract;
using Page = PriceHunter.Common.Pager.Page;

namespace PriceHunter.Business.User.Concrete
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<PriceHunter.Model.User.User> _userRepository;
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        FileConfigurationOptions _fileConfigurationOptions;

        public UserService(
            IGenericRepository<Model.User.User> userRepository,
            ICacheService cacheService,
            ILockService lockService,
            IMapper mapper,
            IValidationService validationService,
            IConfiguration configuration,
            ILogger<UserService> logger,
            IOptions<FileConfigurationOptions> fileConfigurationOptions)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _lockService = lockService;
            _mapper = mapper;
            _validationService = validationService;
            _configuration = configuration;
            _logger = logger;
            _fileConfigurationOptions = fileConfigurationOptions.Value;
        }

        #region CRUD Operations

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

            var response = _mapper.Map<UserViewModel>(user);
            response.Image = string.IsNullOrWhiteSpace(user.Image) ? user.Image : Path.Combine(_fileConfigurationOptions.UserProfileVirtualPath, user.Image);

            return new ServiceResult<UserViewModel>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = response
            };
        }
        public async Task<ServiceResult<ExpandoObject>> CreateAsync(CreateUserRequestServiceRequest request)
        {
            var validationResponse = _validationService.Validate(typeof(CreateUserRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            request.Email = request.Email.Trim().ToLower();
            if (await _userRepository.AnyAsync(p => p.Email.Equals(request.Email) && p.IsDeleted == false))
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = Resource.Duplicate(request.Email)
                };
            }

            var entity = new Model.User.User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Type = Model.User.UserType.Root //TODO: this need to be change as your logic                
            };

            entity = await _userRepository.InsertAsync(entity);

            dynamic userWrapper = new ExpandoObject();
            userWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Created(Entities.User, entity.Email),
                Data = userWrapper
            };
        }
        public async Task<ServiceResult<ExpandoObject>> UpdateAsync(UpdateUserRequestServiceRequest request)
        {
            var validationResponse = _validationService.Validate(typeof(UpdateUserRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            var entity = await _userRepository.FindOneAsync(p => p.Id == request.Id && p.IsDeleted == false);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.User)
                };
            }

            request.Email = request.Email.Trim().ToLower();
            if (await _userRepository.AnyAsync(p => p.Id != request.Id && p.Email.Equals(request.Email) && p.IsDeleted == false))
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = Resource.Duplicate(entity.Email)
                };
            }

            var lockKey = string.Format(LockKeyConstants.UserLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.UserCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey))
            {
                entity.FirstName = request.FirstName.Trim();
                entity.LastName = request.LastName.Trim();
                entity.Email = request.Email;

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    entity.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                entity = await _userRepository.UpdateAsync(entity);

                await _cacheService.RemoveAsync(cacheKey);
            }


            dynamic userWrapper = new ExpandoObject();
            userWrapper.Id = entity.Id;

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Updated(Entities.User, entity.Email),
                Data = userWrapper
            };
        }
        public async Task<ServiceResult<UserProfileImageViewModel>> UploadProfileImageAsync(ProfileFileContractServiceRequest request)
        {
            var validationResponse = _validationService.Validate(typeof(ProfileFileContractServiceRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<UserProfileImageViewModel>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            var entity = await _userRepository.FindOneAsync(p => p.Id == request.Id && p.IsDeleted == false);

            if (entity == null)
            {
                return new ServiceResult<UserProfileImageViewModel>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.User)
                };
            }

            var lockKey = string.Format(LockKeyConstants.UserLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.UserCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey))
            {
                DeleteImage(entity.Image);
                entity.Image = await UploadImageAsync(request.FileName, request.FileData);

                entity = await _userRepository.UpdateAsync(entity);

                await _cacheService.RemoveAsync(cacheKey);
            }

            var response = new UserProfileImageViewModel();
            response.Image =  Path.Combine(_fileConfigurationOptions.UserProfileVirtualPath, entity.Image);

            return new ServiceResult<UserProfileImageViewModel>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Updated(Entities.User, entity.Email),
                Data = response
            };
        }
        public async Task<ServiceResult<ExpandoObject>> DeleteAsync(Guid id)
        {
            var entity = await _userRepository.FindOneAsync(p => p.Id == id && p.IsDeleted == false);

            if (entity == null)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.User)
                };
            }

            if (entity.Id == ApplicationContext.Instance.CurrentUser.Id)
            {
                return new ServiceResult<ExpandoObject>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = ServiceResponseMessage.CANNOT_DELETE_ACTIVE_USER
                };
            }

            var lockKey = string.Format(LockKeyConstants.UserLockKey, entity.Id);
            var cacheKey = string.Format(CacheKeyConstants.UserCacheKey, entity.Id);

            using (await _lockService.CreateLockAsync(lockKey))
            {
                await _userRepository.DeleteAsync(entity);
                DeleteImage(entity.Image);

                await _cacheService.RemoveAsync(cacheKey);
            }

            return new ServiceResult<ExpandoObject>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Deleted(Entities.User, entity.Email)
            };
        }
        public async Task<ServiceResult<PagedList<UserViewModel>>> SearchAsync(FilteryRequest request)
        {
            var filteryResponse = await _userRepository.Find(p => true).BuildFilteryAsync(new UserFilteryMapping(), request);

            var response = new PagedList<UserViewModel>
            {
                Data = _mapper.Map<List<UserViewModel>>(filteryResponse.Data),
                PageInfo = new Page
                {
                    PageNumber = filteryResponse.PageNumber,
                    PageSize = filteryResponse.PageSize,
                    TotalItemCount = filteryResponse.TotalItemCount
                }
            };

            response.Data.ForEach(p => p.Image = string.IsNullOrWhiteSpace(p.Image) ? p.Image : Path.Combine(_fileConfigurationOptions.UserProfileVirtualPath, p.Image));

            return new ServiceResult<PagedList<UserViewModel>>
            {
                Data = response,
                Status = ResultStatus.Successful
            };
        }

        #endregion


        #region Login Operations

        public async Task<ServiceResult<AccessTokenContract>> GetTokenAsync(GetTokenContractServiceRequest request)
        {
            var validationResponse = _validationService.Validate(typeof(GetTokenContractServiceRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<AccessTokenContract>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            request.Email = request.Email.Trim().ToLower();
            var entity = await _userRepository.FindOneAsync(p => p.Email.Equals(request.Email) && p.IsDeleted == false);

            if (entity == null)
            {
                return new ServiceResult<AccessTokenContract>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(request.Email)
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, entity.Password))
            {
                return new ServiceResult<AccessTokenContract>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(request.Email)
                };
            }

            var token = new JwtManager().GenerateToken(new JwtContract
            {
                Id = entity.Id,
                Name = $"{entity.FirstName} {entity.LastName}",
                Email = entity.Email,
                UserType = entity.Type,
            });

            entity.RefreshToken = token.RefreshToken;
            entity.RefreshTokenExpireDate = token.RefreshTokenExpireDate;

            await _userRepository.UpdateAsync(entity);

            return new ServiceResult<AccessTokenContract>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = token
            };
        }
        public async Task<ServiceResult<AccessTokenContract>> RefreshTokenAsync(RefreshTokenContractServiceRequest request)
        {
            var validationResponse = _validationService.Validate(typeof(RefreshTokenContractServiceRequestValidator), request);

            if (!validationResponse.IsValid)
            {
                return new ServiceResult<AccessTokenContract>
                {
                    Status = ResultStatus.InvalidInput,
                    Message = ServiceResponseMessage.INVALID_INPUT_ERROR,
                    ValidationMessages = validationResponse.ErrorMessages
                };
            }

            var entity = await _userRepository.FindOneAsync(p =>
                p.RefreshToken == request.Token &&
                p.RefreshTokenExpireDate > DateTime.UtcNow &&
                p.IsDeleted == false);

            if (entity == null)
            {
                return new ServiceResult<AccessTokenContract>
                {
                    Status = ResultStatus.ResourceNotFound,
                    Message = Resource.NotFound(Entities.User)
                };
            }

            var token = new JwtManager().GenerateToken(new JwtContract
            {
                Id = entity.Id,
                Name = $"{entity.FirstName} {entity.LastName}",
                Email = entity.Email,
                UserType = entity.Type
            });

            entity.RefreshToken = token.RefreshToken;
            entity.RefreshTokenExpireDate = token.RefreshTokenExpireDate;

            await _userRepository.UpdateAsync(entity);

            return new ServiceResult<AccessTokenContract>
            {
                Status = ResultStatus.Successful,
                Message = Resource.Retrieved(),
                Data = token
            };
        }

        #endregion

        private async Task<string> UploadImageAsync(string fileName, byte[] image)
        {
            string newFileName = null;
            if (image != null)
            {
                try
                {
                    var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), _fileConfigurationOptions.UserProfilePhysicalPath);

                    if (!Directory.Exists(imageFolderPath))
                    {
                        Directory.CreateDirectory(imageFolderPath);
                    }

                    FileInfo imageFileInfo = new FileInfo(fileName);

                    newFileName = $"{Guid.NewGuid().ToString()}{imageFileInfo.Extension}";
                    var fileFullPath = Path.Combine(imageFolderPath, newFileName);

                    await File.WriteAllBytesAsync(fileFullPath, image);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
            return newFileName;
        }
        private void DeleteImage(string imageName)
        {
            if (!string.IsNullOrWhiteSpace(imageName))
            {
                var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), _fileConfigurationOptions.UserProfilePhysicalPath);
                var oldImageFileFullPath = Path.Combine(imageFolderPath, imageName);
                if (File.Exists(oldImageFileFullPath))
                {
                    File.Delete(oldImageFileFullPath);
                }
            }
        }
    }
}
