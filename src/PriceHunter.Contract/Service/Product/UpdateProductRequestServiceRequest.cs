namespace PriceHunter.Contract.Service.Product
{
    public class UpdateProductRequestServiceRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public List<ProductSupplierInfoMappingServiceModel> UrlSupplierMapping { get; set; }
    }
}
