using System.ComponentModel.DataAnnotations;

namespace PriceHunter.Web.Data.UserProduct.App
{
    public class UrlSupplierMappingViewModel
    {
        [Url]
        [MaxLength(1000)]
        public string Url { get; set; }
        public string Supplier { get; set; }
        public Guid SupplierId { get; set; }
    }
}
