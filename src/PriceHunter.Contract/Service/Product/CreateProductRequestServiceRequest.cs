namespace PriceHunter.Contract.Service.Product
{
    public class CreateProductRequestServiceRequest
    {
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public List<ProductSupplierInfoMappingServiceModel> UrlSupplierMapping { get; set; }
    }
}
