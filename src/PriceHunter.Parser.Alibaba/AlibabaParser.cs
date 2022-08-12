namespace PriceHunter.Parser.Alibaba
{
    public class AlibabaParser : IParser
    {
        public double Parse(string productUrl)
        {
            return new Random().Next(1,600);
        }
    }
}
