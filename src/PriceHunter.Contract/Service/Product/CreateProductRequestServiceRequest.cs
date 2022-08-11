namespace PriceHunter.Contract.Service.Product
{
    public class CreateProductRequestServiceRequest
    {
        public string Name { get; set; }
        public List<ProductSupplierInfoMappingServiceModel> UrlSupplierMapping { get; set; }
    }
}
