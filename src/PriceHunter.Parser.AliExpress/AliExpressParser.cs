namespace PriceHunter.Parser.AliExpress
{
    public class AliExpressParser : IParser
    {
        public double Parse(string productUrl)
        {
            return new Random().Next(1, 600);
        }
    }
}
