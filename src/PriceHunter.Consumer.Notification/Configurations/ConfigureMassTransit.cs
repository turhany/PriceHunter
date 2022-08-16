using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceHunter.Common.Constans;
using PriceHunter.Common.Options;
using PriceHunter.Consumer.Notification.Consumers;

namespace PriceHunter.Consumer.Notification.Configurations
{
    /// <summary>
    /// Database configuration extension
    /// </summary>
    public static class ConfigureMassTransit
    {
        /// <summary>
        /// Add MassTransit configurations
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransitConfigurationForConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOption>(configuration.GetSection(AppConstants.RabbitMqSettingsOptionName));

            var rabbitMqConfig = new RabbitMqOption();
            configuration.GetSection(AppConstants.RabbitMqSettingsOptionName).Bind(rabbitMqConfig);

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqConfig.RabbitMqUri), hst =>
                    {
                        hst.Username(rabbitMqConfig.UserName);
                        hst.Password(rabbitMqConfig.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<NotificationConsumer>(opts =>
                {
                    opts.UseConcurrencyLimit(1);
                    opts.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });
                }).Endpoint(endpoint => { endpoint.Name = rabbitMqConfig.NotificationQueue; });
            });

            return services;
        }
    }
}
