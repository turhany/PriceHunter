using Autofac.Extensions.DependencyInjection;
using Autofac;
using PriceHunter.Container.Modules;
using PriceHunter.Common.StartupConfigurations; 
using PriceHunter.ScheduleService.Configurations;
using PriceHunter.Contract.Mappings.AutoMapper;
using PriceHunter.Common.Application;
using PriceHunter.ScheduleService.Schedules;
using PriceHunter.Common.Constans;
using PriceHunter.Cache.Redis.StartupConfigurations;
using PriceHunter.Lock.Redis.StartupConfigurations;
using PriceHunter.Data.MongoDB.StartupConfigurations;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new ApplicationModule());
        builder.RegisterModule(new RepositoryModule());
        builder.RegisterModule(new ServiceModule());
        builder.RegisterModule(new ParserModule());
        builder.RegisterModule(new NotificationModule());
    });

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDistributedCacheConfiguration(builder.Configuration.GetConnectionString(AppConstants.RedisConnectionString), AppConstants.RedisCacheInstanceName);
builder.Services.AddDistributedLockConfiguration(builder.Configuration, AppConstants.RedLockSettingsOptionName);
builder.Services.AddMongoDBConfiguration(builder.Configuration, AppConstants.MongoSettingsOptionName);
builder.Services.AddHangfireConfiguration(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMassTransitConfiguration(builder.Configuration);
builder.Services.AddAutoMapper(typeof(ProductMapping));
 
var app = builder.Build();
app.UseRouting();
app.UseHangfireConfiguration();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await RecurringJobs.CheckProductPricesAsync(app.Services.GetRequiredService<IServiceScopeFactory>());

ApplicationContext.ConfigureWorkerServiceUser(Guid.Parse(builder.Configuration["Application:ServiceUserId"]));
ApplicationContext.ConfigureThreadPool(builder.Configuration);

app.Run();