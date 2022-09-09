namespace PriceHunter.Parser.Models
{
    public class ParseResponse
    {
        public decimal Price { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public dynamic ExtraData { get; set; }
    }
}
