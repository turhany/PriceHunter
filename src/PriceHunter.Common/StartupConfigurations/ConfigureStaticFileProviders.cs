using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Options;

namespace PriceHunter.Common.StartupConfigurations
{
    /// <summary>
    /// Static file configuration extension
    /// </summary>
    public static class ConfigureStaticFileProviders
    {

        /// <summary>
        /// Add Static file configuration
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddStaticFileConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FileConfigurationOptions>(configuration.GetSection(AppConstants.FileConfigurationsOptionName));

            return services;
        }

        /// <summary>
        /// Use Static file configuration
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticFileConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        { 
            var fileConfigurations = new FileConfigurationOptions();
            configuration.GetSection(AppConstants.FileConfigurationsOptionName).Bind(fileConfigurations);
             
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), fileConfigurations.UserProfilePhysicalPath)),
                RequestPath = new PathString(fileConfigurations.UserProfileVirtualPath)
            });
            return app;
        }
    }
}