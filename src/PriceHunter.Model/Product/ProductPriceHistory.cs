using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Product
{
    public class ProductPriceHistory : SoftDeleteEntity
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
