using Hangfire;
using PriceHunter.ScheduleService.Filters;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;

namespace PriceHunter.ScheduleService.Configurations
{
    public static class ConfigureHangfire
    {
        /// <summary>
        /// Add Health check configuration extension
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var mongoUrlBuilder = new MongoUrlBuilder($"{configuration["MongoSettings:ConnectionString"]}/{configuration["MongoSettings:Database"]}");
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "Hangfire",
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                })
            );

            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.ServerName = $"Server-{Environment.MachineName}";
            });


            return services;
        }

        /// <summary>
        /// Use Health check configuration
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHangfireConfiguration(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "PriceHunter HangFire DashBoard",
                AppPath = "/hangfire",
                IgnoreAntiforgeryToken = true,
                Authorization = new []{ new DashboardNoAuthorizationFilter() }
            });

            return app;
        }
    }
}