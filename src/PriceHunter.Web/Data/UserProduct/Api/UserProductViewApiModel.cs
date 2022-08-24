namespace PriceHunter.Web.Data.UserProduct.Api
{
    public class UserProductViewApiModel
    {
        public string Name { get; set; }
        public List<UrlSupplierMappingViewApiModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewApiModel>();
    }
}
