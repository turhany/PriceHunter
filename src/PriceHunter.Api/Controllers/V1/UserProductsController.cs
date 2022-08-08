using Microsoft.AspNetCore.Mvc; 
using PriceHunter.Business.UserProduct.Abstract;
using PriceHunter.Common.BaseModels.Api;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Service.UserProduct;

namespace PriceHunter.Api.Controllers.V1
{
    /// <summary>
    /// User Products Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class UserProductsController : BaseController
    {
        private readonly IUserProductService _userProductService;

        /// <summary>
        /// User Products Controller
        /// </summary>
        /// <param name="userProductService"></param>
        public UserProductsController(IUserProductService userProductService)
        {
            _userProductService = userProductService;
        }

        /// <summary>
        /// Get User Product
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProductViewModel))]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _userProductService.GetAsync(id);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Create User Product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateProduct([FromBody] CreateUserProductRequest request)
        {
            if (request == null) return ApiResponse.InvalidInputResult;

            var result = await _userProductService.CreateAsync(Mapper.Map<CreateUserProductRequestServiceRequest>(request));
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Update User Product
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateUserProductRequest request, Guid id)
        {
            if (request == null) return ApiResponse.InvalidInputResult;
            var model = Mapper.Map<UpdateUserProductRequestServiceRequest>(request);
            model.Id = id;

            var result = await _userProductService.UpdateAsync(model);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Delete User Product
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
                return ApiResponse.InvalidInputResult;

            var result = await _userProductService.DeleteAsync(id);
            return ApiResponse.CreateResult(result);
        }
    }
}
