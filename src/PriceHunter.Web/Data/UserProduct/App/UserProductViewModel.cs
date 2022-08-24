namespace PriceHunter.Web.Data.UserProduct.App
{
    public class UserProductViewModel
    {
        public string Name { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
    }
}
