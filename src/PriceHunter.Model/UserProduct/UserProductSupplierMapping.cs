using PriceHunter.Common.Data; 

namespace PriceHunter.Model.UserProduct
{
    public class UserProductSupplierMapping : SoftDeleteEntity
    {
        public Guid UserProductId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? ProductId { get; set; }
        public string Url { get; set; }
        public Guid UserId { get; set; }
    }
}
