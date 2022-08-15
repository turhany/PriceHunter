using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Product
{
    public class ProductPriceHistory : SoftDeleteEntity
    {
        public double Price { get; set; }
        public Guid ProductSupplierInfoMappingId { get; set; }
    }
}
