using PriceHunter.Common.Extensions;
using PriceHunter.Parser.Models;
using PuppeteerSharp;

namespace PriceHunter.Parser.Alibaba
{
    public class AlibabaParser : IParser
    {
        public async Task<ParseResponse> ParseAsync(string productUrl, List<string> priceParseScripts)
        {
            var response = new ParseResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(productUrl) || priceParseScripts == null || !priceParseScripts.Any())
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Product url or Price Parse Scripts not found.");

                    return response;
                }

                using var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });

                using (var page = await browser.NewPageAsync())
                {
                    page.DefaultTimeout = 0;
                    var navigation = new NavigationOptions
                    {
                        Timeout = 0,
                        WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
                    };
                    await page.GoToAsync(productUrl, navigation);

                    foreach (var priceParseScript in priceParseScripts)
                    {
                        try
                        {
                            var priceParseResult = await page.EvaluateFunctionAsync<dynamic>(priceParseScript);
                            var parsedPriceText = $"{priceParseResult.price}{priceParseResult.divider}{priceParseResult.fraction}";

                            response.Price = parsedPriceText.ConvertToDecimal();
                            response.IsSuccess = true;

                            break;
                        }
                        catch (Exception ex)
                        {
                            response.ErrorMessages.Add(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add(ex.Message);
            }

            if (response.IsSuccess && response.Price == 0)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("No price found.");
            }

            return response;
        }
    }
}
