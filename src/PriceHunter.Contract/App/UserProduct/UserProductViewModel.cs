namespace PriceHunter.Contract.App.UserProduct
{
    public class UserProductViewModel
    {
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
    }
}
