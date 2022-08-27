using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceHunter.Lock.Options;

namespace PriceHunter.Lock.Redis.StartupConfigurations
{
    /// <summary>
    /// Distributed lock configuration extension
    /// </summary>
    public static class ConfigureDistributedLock
    {
        /// <summary>
        /// Add Distributed lock configuration
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLockConfiguration(this IServiceCollection services, IConfiguration configuration, string configSectionName)
        {
            services.Configure<RedLockOption>(configuration.GetSection(configSectionName));
            
            return services;
        } 
    }
}