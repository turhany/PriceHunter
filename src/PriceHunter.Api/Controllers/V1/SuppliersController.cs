using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using PriceHunter.Business.Supplier.Abstract;
using PriceHunter.Common.BaseModels.Api; 
using PriceHunter.Contract.App.Supplier;

namespace PriceHunter.Api.Controllers.V1
{
    /// <summary>
    /// Suppliers Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class SuppliersController : BaseController
    {
        private readonly ISupplierService _supplierService;

        /// <summary>
        /// Suppliers Controller
        /// </summary>
        /// <param name="supplierService"></param>
        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        /// <summary>
        /// Get Suppliers
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierViewModel))]
        public async Task<ActionResult> All(CancellationToken cancellationToken)
        {
            var result = await _supplierService.GetAllAsync(cancellationToken);
            return ApiResponse.CreateResult(result);
        }
    }
}
