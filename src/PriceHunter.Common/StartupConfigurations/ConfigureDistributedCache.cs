﻿using PriceHunter.Common.Constans;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceHunter.Common.Lock.Concrete;

namespace PriceHunter.Common.StartupConfigurations
{
    /// <summary>
    /// Distributed cache configuration extension
    /// </summary>
    public static class ConfigureDistributedCache
    {
        /// <summary>
        /// Add Distributed cache configuration
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedCacheConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedLockOption>(configuration.GetSection(AppConstants.RedLockSettingsOptionName));

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString(AppConstants.RedisConnectionString);
                options.InstanceName = AppConstants.RedisCacheInstanceName;
            });
            return services;
        } 
    }
}