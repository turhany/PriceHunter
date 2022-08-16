using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Product
{
    public class ProductPriceHistory : SoftDeleteEntity
    {        
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public double Price { get; set; }
    }
}
