namespace PriceHunter.Contract.App.Product
{
    public class ProductPriceChangesViewModel
    {
        public decimal Price { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid SupplierId { get; set; }
    }
}
