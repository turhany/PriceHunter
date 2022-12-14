namespace PriceHunter.Contract.Service.UserProduct
{
    public class UpdateUserProductRequestServiceRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public bool IsFavorite { get; set; }
        public List<UrlSupplierMappingServiceModel> UrlSupplierMapping { get; set; }
    }
}
