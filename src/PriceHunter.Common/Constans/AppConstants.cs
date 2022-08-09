namespace PriceHunter.Common.Constans
{
    public static class AppConstants
    {
        public const string ProductName = "PriceHunter";
        public const string JsonContentType = "application/json";
        
        public const string RedisConnectionString = "RedisConnectionString";
        
        public const string RedisCacheInstanceName = "PriceHunterCache";
        public const int DefaultCacheDuration = 10;
        
        public const string HashKey = "turhany";
         
        public const string RabbitMqSettingsOptionName = "RabbitMqSettings";
        public const string RedLockSettingsOptionName = "RedLockSettings";
        public const string IPRateLimitingOptionName = "RateLimiting:IpRateLimiting";
        public const string RateLimitingModeOptionName = "RateLimiting:RateLimitingMode";

        public const string ClaimTypesId = "Id";
        public const string SymmetricKey = "!#_PriceHunter_Symmetric_Key_2022_!#";
        public const double AccessTokenExpireMinute = 6 * 60;//6 hour
        public const double RefreshTokenExpireMinute = 7 * 60; //7 hour
    }
}