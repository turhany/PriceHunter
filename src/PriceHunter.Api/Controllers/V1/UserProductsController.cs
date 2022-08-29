using Filtery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using PriceHunter.Business.UserProduct.Abstract;
using PriceHunter.Common.BaseModels.Api;
using PriceHunter.Contract.App.Product;
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
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProductViewModel))]
        public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userProductService.GetAsync(id, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Create User Product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateProduct([FromBody] CreateUserProductRequest request, CancellationToken cancellationToken)
        {
            if (request == null) return ApiResponse.InvalidInputResult;

            var result = await _userProductService.CreateAsync(Mapper.Map<CreateUserProductRequestServiceRequest>(request), cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Update User Product
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateUserProductRequest request, Guid id, CancellationToken cancellationToken)
        {
            if (request == null) return ApiResponse.InvalidInputResult;
            var model = Mapper.Map<UpdateUserProductRequestServiceRequest>(request);
            model.Id = id;

            var result = await _userProductService.UpdateAsync(model, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Delete User Product
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return ApiResponse.InvalidInputResult;

            var result = await _userProductService.DeleteAsync(id, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// User Product Search
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProductSearchViewModel))]
        public async Task<ActionResult> Search([FromBody] FilteryRequest request, CancellationToken cancellationToken)
        {
            var result = await _userProductService.SearchAsync(request, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Get Product
        /// </summary>
        [HttpGet("last6monthchanges/{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductPriceChangesViewModel))]
        public async Task<ActionResult> Last6MonthChanges(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userProductService.GetLastNMonthChangesAsync(id, 6, cancellationToken);
            return ApiResponse.CreateResult(result);
        }
    }
}
