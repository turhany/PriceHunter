using System.ComponentModel.DataAnnotations;

namespace PriceHunter.Web.Data.UserProduct.App
{
    public class UserProductViewModel
    {
        [Required]
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public string Currency { get; set; }
        public string CurrencyShortCode { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
    }
}
