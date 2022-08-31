using Autofac;
using Autofac.Extensions.DependencyInjection;
using PriceHunter.Api.Configurations.Startup;
using PriceHunter.Api.Middlewares;
using PriceHunter.Cache.Redis.StartupConfigurations;
using PriceHunter.Common.Application;
using PriceHunter.Common.Constans;
using PriceHunter.Common.StartupConfigurations;
using PriceHunter.Container.Modules;
using PriceHunter.Contract.Mappings.AutoMapper; 
using PriceHunter.Data.MongoDB.StartupConfigurations;
using PriceHunter.Lock.Redis.StartupConfigurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new ApplicationModule());
        builder.RegisterModule(new RepositoryModule());
        builder.RegisterModule(new ServiceModule());
    })
    .UseSerilog((context, conf) =>
        conf.ReadFrom.Configuration(context.Configuration)
            .Enrich.WithCorrelationId()
            .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("swagger"))));


builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddIdentityConfigurations(builder.Configuration);
builder.Services.AddLocalizationsConfigurations();
builder.Services.AddDistributedCacheConfiguration(builder.Configuration.GetConnectionString(AppConstants.RedisConnectionString), AppConstants.RedisCacheInstanceName);
builder.Services.AddDistributedLockConfiguration(builder.Configuration, AppConstants.RedLockSettingsOptionName);
builder.Services.AddMongoDBConfiguration(builder.Configuration, AppConstants.MongoSettingsOptionName); 
builder.Services.AddStaticFileConfiguration(builder.Configuration);
builder.Services.AddCorsConfigurations();
builder.Services.AddCompressionConfiguration();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddApiVersioningConfigurations();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMassTransitConfiguration(builder.Configuration);
builder.Services.AddHealthCheckConfiguration(builder.Configuration);
builder.Services.AddRateLimitingConfiguration(builder.Configuration);
builder.Services.AddAutoMapper(typeof(ProductMapping));


var app = builder.Build();
app.UseMiddleware<RequestLogMiddleware>();
app.UseLocalizationConfiguration();
app.UseSwaggerConfiguration();
app.UseHealthCheckConfiguration();
app.UseSecuritySettings();
app.UseRouting();
app.UseCompressionConfiguration();
app.UseStaticFileConfiguration(builder.Configuration);
app.UseCorsConfiguration();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseRateLimitingConfiguration(builder.Configuration);
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


ApplicationContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
ApplicationContext.ConfigureThreadPool(builder.Configuration);

app.Run();