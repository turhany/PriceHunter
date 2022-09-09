using PriceHunter.Parser.Models;

namespace PriceHunter.Parser
{
    public interface IParser
    {
        Task<ParseResponse> ParseAsync(string productUrl, List<string> priceParseScripts);
    }
}
