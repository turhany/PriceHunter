using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using PriceHunter.Common.Application;
using PriceHunter.Container.Modules;
using PriceHunter.Contract.Mappings.AutoMapper;
using Serilog;
using PriceHunter.Consumer.Parser.Configurations; 
using PriceHunter.Consumer.Parser;
using PriceHunter.Common.Constans;
using PriceHunter.Cache.Redis.StartupConfigurations;
using PriceHunter.Lock.Redis.StartupConfigurations;
using PriceHunter.Data.MongoDB.StartupConfigurations;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var hostBuilder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(x => x.AddJsonFile($"appsettings.{env}.json", true, true))
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new ApplicationModule());
        builder.RegisterModule(new RepositoryModule());
        builder.RegisterModule(new ServiceModule());
        builder.RegisterModule(new ParserModule());
    })
    .UseSerilog((context, conf) => conf.ReadFrom.Configuration(context.Configuration))
    .ConfigureServices((hostingContext, services) =>
    {
        services.AddMongoDBConfiguration(hostingContext.Configuration, AppConstants.MongoSettingsOptionName); services.AddDistributedCacheConfiguration(hostingContext.Configuration.GetConnectionString(AppConstants.RedisConnectionString), AppConstants.RedisCacheInstanceName);
        services.AddDistributedLockConfiguration(hostingContext.Configuration, AppConstants.RedLockSettingsOptionName);
        services.AddMassTransitConfigurationForConsumer(hostingContext.Configuration);
        services.AddAutoMapper(typeof(ProductMapping));
        services.AddHostedService<QueueWorker>();
    });

hostBuilder.UseConsoleLifetime();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    hostBuilder.UseSystemd();
else
    hostBuilder.UseWindowsService();

var configFile = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{env}.json", optional: false)
    .Build();

ApplicationContext.ConfigureWorkerServiceUser(Guid.Parse(configFile["Application:ServiceUserId"]));
ApplicationContext.ConfigureThreadPool(configFile);

hostBuilder.Build().Run();