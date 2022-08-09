using PriceHunter.Common.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PriceHunter.Api.Configurations.Startup
{
    /// <summary>
    /// Configure Identity Server
    /// </summary>
    public static class ConfigureIdentityServer
    {
        /// <summary>
        /// Add Identity Configurations
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = JwtManager.ValidationParameters;
            });
            
            return services;
        }
    }
}