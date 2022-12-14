using Filtery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceHunter.Business.Product.Abstract;
using PriceHunter.Common.BaseModels.Api;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Service.Product;

namespace PriceHunter.Api.Controllers.V1
{
    /// <summary>
    /// Products Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Products Controller
        /// </summary>
        /// <param name="productService"></param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get Product
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductViewModel))]
        public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.GetAsync(id, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Create Product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            if (request == null) return ApiResponse.InvalidInputResult;

            var result = await _productService.CreateAsync(Mapper.Map<CreateProductRequestServiceRequest>(request), cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductRequest request, Guid id, CancellationToken cancellationToken)
        {
            if (request == null) return ApiResponse.InvalidInputResult;
            var model = Mapper.Map<UpdateProductRequestServiceRequest>(request);
            model.Id = id;

            var result = await _productService.UpdateAsync(model, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return ApiResponse.InvalidInputResult;

            var result = await _productService.DeleteAsync(id, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Product Search
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductSearchViewModel))]
        public async Task<ActionResult> Search([FromBody] FilteryRequest request, CancellationToken cancellationToken)
        {
            var result = await _productService.SearchAsync(request, cancellationToken);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Product Search
        /// </summary>
        /// <returns></returns>
        [HttpPost("pricehistory/search")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductPriceHistorySearchViewModel))]
        public async Task<ActionResult> SearchPriceHistory([FromBody] FilteryRequest request, CancellationToken cancellationToken)
        {
            var result = await _productService.SearchPriceHistoryAsync(request, cancellationToken);
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
            var result = await _productService.GetLastNMonthChangesAsync(id, 6, cancellationToken);
            return ApiResponse.CreateResult(result);
        }
    }
}
