namespace PriceHunter.Contract.App.Product
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; } = new List<ProductSupplierInfoMappingViewModel>();
    }
}
