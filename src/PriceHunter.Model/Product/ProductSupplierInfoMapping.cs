using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Product
{
    public class ProductSupplierInfoMapping : SoftDeleteEntity
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public string Url { get; set; }
    }
}
