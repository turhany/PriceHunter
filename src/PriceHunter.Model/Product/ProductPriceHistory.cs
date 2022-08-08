using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Product
{
    public class ProductPriceHistory : SoftDeleteEntity
    {
        public Guid ProductSupplierInfoMapping { get; set; }
    }
}
