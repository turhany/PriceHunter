namespace PriceHunter.Web.Data.UserProduct.App
{
    public class ProductPriceChangesViewModel
    {
        public double Price { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid SupplierId { get; set; }
        public string Supplier { get; set; }
    }
}
