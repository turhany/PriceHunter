namespace PriceHunter.Contract.App.Product
{
    public class ProductPriceHistorySearchViewModel
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public double Price { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public TimeSpan Time { get; set; }
    }
}
