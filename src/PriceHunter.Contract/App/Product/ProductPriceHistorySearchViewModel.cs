namespace PriceHunter.Contract.App.Product
{
    public class ProductPriceHistorySearchViewModel
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public double Price { get; set; }
    }
}
