using PriceHunter.Business.TestData.Abstract; 

namespace PriceHunter.Api.Configurations.Startup
{
    /// <summary>
    /// Add test data extension
    /// </summary>
    public static class ConfigureTestData
    {
        /// <summary>
        /// Insert Test data extension
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="testDataService">Test Data Service</param>
        /// <returns></returns>
        public static IApplicationBuilder InsertTestData(this IApplicationBuilder app, ITestDataService testDataService)
        {
            testDataService.InsertDataAsync(CancellationToken.None).Wait();

            return app;
        }
    }
}