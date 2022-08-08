using Microsoft.AspNetCore.Mvc;
using PriceHunter.Business.User.Abstract;
using PriceHunter.Common.BaseModels.Api;
using PriceHunter.Contract.App.User;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserViewModel))]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _userService.GetAsync(id);
            return ApiResponse.CreateResult(result);
        }
    }
}
