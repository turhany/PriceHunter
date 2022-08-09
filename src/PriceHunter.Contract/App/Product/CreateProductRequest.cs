namespace PriceHunter.Contract.App.Product
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; }
    }
}
