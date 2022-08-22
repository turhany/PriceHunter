namespace PriceHunter.Web.Helpers.Constants
{
    public class AppConstants
    {
        public static readonly string V1ApiTokenUrl = "http://localhost:5010/api/v1/login/token";
        public static readonly string V1UserUrl = "http://localhost:5010/api/v1/users/{0}";

        public static readonly string TokenStorageKey = "tokenData";

        public static readonly string AuthenticationType = "jwt";
    }
}
