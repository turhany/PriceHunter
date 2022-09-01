namespace PriceHunter.Web.Data.UserProduct.Api
{
    public class UserProductCreateApiModel
    {
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public bool IsFavorite { get; set; }
        public List<UrlSupplierMappingCreateModel> UrlSupplierMapping { get; set; }
    }
}
