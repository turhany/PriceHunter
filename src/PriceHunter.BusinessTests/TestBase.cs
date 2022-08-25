using Autofac;
using AutoMapper;
using PriceHunter.Common.Application;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using PriceHunter.Common.Lock.Abstract;
using PriceHunter.Container.Modules;
using PriceHunter.Contract.Mappings.AutoMapper;
using Microsoft.Extensions.Configuration;
using Mongo2Go;
using Microsoft.Extensions.Options;
using PriceHunter.Data.MongoDB.Options;
using PriceHunter.Common.Options;
using Microsoft.Extensions.Logging;
using PriceHunter.Business.User.Concrete;

namespace PriceHunter.BusinessTests
{
    public class TestBase
    {
        protected IContainer Container { get; }

        internal static MongoDbRunner _runner;

        public TestBase()
        {
            //Autofac DI configuration
            var builder = new ContainerBuilder();

            var mockDistributedCache = new Mock<IDistributedCache>();
            mockDistributedCache.Setup(p => p.GetAsync(It.IsAny<string>(), default(CancellationToken))).ReturnsAsync(()=> null);
            
            builder.RegisterInstance(mockDistributedCache.Object).As<IDistributedCache>();

            _runner = MongoDbRunner.Start();
            MongoDBOption mongoDBOption = new MongoDBOption() { 
                ConnectionString = _runner.ConnectionString,
                Database = "PriceHunterDB_Test"
            }; 
                                                                                            
            var mockMongoOptions = new Mock<IOptions<MongoDBOption>>();
            mockMongoOptions.Setup(ap => ap.Value).Returns(mongoDBOption);
            builder.RegisterInstance(mockMongoOptions.Object).As<IOptions<MongoDBOption>>();

            var mockLogger = new Mock<ILogger<UserService>>(); 
            builder.RegisterInstance(mockLogger.Object).As<ILogger<UserService>>();

            var mockFileConfigurationOptions = new Mock<IOptions<FileConfigurationOptions>>();
            builder.RegisterInstance(mockFileConfigurationOptions.Object).As<IOptions<FileConfigurationOptions>>();

            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new RepositoryModule());
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new NotificationModule());
            builder.RegisterModule(new ParserModule());

            var mockLock = new Mock<ILockService>();
            builder.RegisterInstance(mockLock.Object).As<ILockService>();
            
            var confLock = new Mock<IConfiguration>();
            builder.RegisterInstance(confLock.Object).As<IConfiguration>();
             
            builder.Register<IMapper>(c =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new UserMapping());
                });
                return config.CreateMapper();
            }).SingleInstance();
            
            Container = builder.Build();
            
            ApplicationContext.ConfigureWorkerServiceUser(Guid.NewGuid());
        }
    }
}