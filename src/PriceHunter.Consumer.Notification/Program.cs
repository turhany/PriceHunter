using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using PriceHunter.Common.Application;
using PriceHunter.Common.StartupConfigurations;
using PriceHunter.Container.Modules;
using PriceHunter.Contract.Mappings.AutoMapper;
using Serilog;
using PriceHunter.Consumer.Notification.Configurations;
using PriceHunter.Data.MongoDB.Options;
using PriceHunter.Consumer.Notification;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var hostBuilder = Host.CreateDefaultBuilder()
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new ApplicationModule());
        builder.RegisterModule(new RepositoryModule());
        builder.RegisterModule(new ServiceModule());
        builder.RegisterModule(new NotificationModule());
    })
    .UseSerilog((context, conf) => conf.ReadFrom.Configuration(context.Configuration))
    .ConfigureServices((hostingContext, services) =>
    {
        services.Configure<MongoDBOption>(hostingContext.Configuration.GetSection("mongo"));
        services.AddDistributedCacheConfiguration(hostingContext.Configuration);
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