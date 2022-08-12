namespace PriceHunter.Parser.Amazon
{
    public class AmazonParser : IParser
    {
        public double Parse(string productUrl)
        {
            return new Random().Next(1, 600);
        }
    }
}
