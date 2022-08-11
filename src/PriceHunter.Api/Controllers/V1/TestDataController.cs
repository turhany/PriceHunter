using Microsoft.AspNetCore.Mvc;
using PriceHunter.Business.TestData.Abstract;

namespace PriceHunter.Api.Controllers.V1
{
    /// <summary>
    /// Test Data Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class TestDataController : BaseController
    {
        private ITestDataService _testDataService;

        /// <summary>
        /// Test Data Controller
        /// </summary>
        /// <param name="testDataService"></param>
        public TestDataController(ITestDataService testDataService)
        {
            _testDataService = testDataService;
        }

        /// <summary>
        /// Insert Test Data
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult> InsertTestData()
        {
            await _testDataService.InsertDataAsync();
            return Ok();
        }

    }
}
