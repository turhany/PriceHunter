namespace PriceHunter.Contract.App.UserProduct
{
    public class UserProductViewModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
    }
}
