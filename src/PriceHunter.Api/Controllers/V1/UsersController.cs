using Filtery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceHunter.Business.User.Abstract;
using PriceHunter.Common.BaseModels.Api;
using PriceHunter.Contract.App.User;
using PriceHunter.Contract.Service.User;

namespace PriceHunter.Api.Controllers.V1
{
    /// <summary>
    /// User Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class UsersController : BaseController
    {
        private IUserService _userService;

        /// <summary>
        /// User Controller
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get User
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserViewModel))]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _userService.GetAsync(id);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateUser([FromBody]CreateUserRequest request)
        {
            if (request == null) return ApiResponse.InvalidInputResult;

            var result = await _userService.CreateAsync(Mapper.Map<CreateUserRequestServiceRequest>(request));
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateUser([FromBody]UpdateUserRequest request, Guid id)
        {
            if (request == null) return ApiResponse.InvalidInputResult;
            var model = Mapper.Map<UpdateUserRequestServiceRequest>(request);
            model.Id = id;

            var result = await _userService.UpdateAsync(model);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <returns></returns>
        [HttpPut("uploadprofileimage/{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UploadProfileImage([FromBody]ProfileFileContract request, Guid id)
        {
            if (request == null) return ApiResponse.InvalidInputResult;
            var model = Mapper.Map<ProfileFileContractServiceRequest>(request);
            model.Id = id;

            var result = await _userService.UploadProfileImageAsync(model);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
                return ApiResponse.InvalidInputResult;

            var result = await _userService.DeleteAsync(id);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// User Search
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserViewModel))]
        public async Task<ActionResult> Search([FromBody] FilteryRequest request)
        {
            var result = await _userService.SearchAsync(request);
            return ApiResponse.CreateResult(result);
        }
    }
}
