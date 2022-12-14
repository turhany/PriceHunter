namespace PriceHunter.Contract.App.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; } 
    }
}
