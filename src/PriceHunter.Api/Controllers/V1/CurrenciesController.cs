using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceHunter.Business.Currency.Abstract;
using PriceHunter.Common.BaseModels.Api;
using PriceHunter.Contract.App.Currency;

namespace PriceHunter.Api.Controllers.V1
{ 
     /// <summary>
    /// Currencies Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class CurrenciesController : BaseController
    {
        private readonly ICurrencyService _currencyService;

        /// <summary>
        /// Currencies Controller
        /// </summary>
        /// <param name="currencyService"></param>
        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// Get Currencies
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyViewModel))]
        public async Task<ActionResult> All(CancellationToken cancellationToken)
        {
            var result = await _currencyService.GetAllAsync(cancellationToken);
            return ApiResponse.CreateResult(result);
        }
    }
}
