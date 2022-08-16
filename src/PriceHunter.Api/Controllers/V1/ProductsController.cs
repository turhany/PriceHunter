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
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _productService.GetAsync(id);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Create Product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (request == null) return ApiResponse.InvalidInputResult;

            var result = await _productService.CreateAsync(Mapper.Map<CreateProductRequestServiceRequest>(request));
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductRequest request, Guid id)
        {
            if (request == null) return ApiResponse.InvalidInputResult;
            var model = Mapper.Map<UpdateProductRequestServiceRequest>(request);
            model.Id = id;

            var result = await _productService.UpdateAsync(model);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
                return ApiResponse.InvalidInputResult;

            var result = await _productService.DeleteAsync(id);
            return ApiResponse.CreateResult(result);
        }

        /// <summary>
        /// Product Search
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Search([FromBody] FilteryRequest request)
        {
            var result = await _productService.SearchAsync(request);
            return ApiResponse.CreateResult(result);
        }

    }
}
