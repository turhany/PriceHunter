using System.ComponentModel.DataAnnotations;

namespace PriceHunter.Web.Data.UserProduct.App
{
    public class UserProductViewModel
    {
        [Required]
        public string Name { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
    }
}
