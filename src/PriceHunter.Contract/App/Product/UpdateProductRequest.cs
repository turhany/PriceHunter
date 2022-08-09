namespace PriceHunter.Contract.App.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; } 
    }
}
