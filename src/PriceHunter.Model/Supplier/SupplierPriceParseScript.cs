using PriceHunter.Common.Data;

namespace PriceHunter.Model.Supplier 
{
    public class SupplierPriceParseScript : SoftDeleteEntity
    {
        public Guid SupplierId { get; set; }
        public string Script { get; set; }
    }
}
