namespace PriceHunter.Web.Helpers.Constants
{
    public class ApiUrls
    {
        public static readonly string V1ApiTokenUrl = "http://localhost:5010/api/v1/login/token";
        public static readonly string V1ApiRefreshTokenUrl = "http://localhost:5010/api/v1/login/refresh-token";

        public static readonly string V1UserUrl = "http://localhost:5010/api/v1/users/{0}";
        public static readonly string V1UserUploadProfileImageUrl = "http://localhost:5010/api/v1/users/uploadprofileimage/{0}";

        public static readonly string V1UserProductUrl = "http://localhost:5010/api/v1/userproducts/{0}";
        public static readonly string V1ApiUserProductSearchUrl = "http://localhost:5010/api/v1/userproducts/search";
        public static readonly string V1UserProductLast6MonthChangesUrl = "http://localhost:5010/api/v1/userproducts/last6monthchanges/{0}";

        public static readonly string V1SupplierAllUrl = "http://localhost:5010/api/v1/suppliers/all";
    }
}
