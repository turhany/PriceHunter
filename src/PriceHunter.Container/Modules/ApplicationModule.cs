using Autofac; 
using PriceHunter.Common.Cache.Abstract;
using PriceHunter.Common.Cache.Concrete;
using PriceHunter.Common.Lock.Abstract;
using PriceHunter.Common.Lock.Concrete;
using PriceHunter.Common.Validation.Abstract;
using PriceHunter.Common.Validation.Concrete;

namespace PriceHunter.Container.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RedisCacheService>().As<ICacheService>().InstancePerLifetimeScope();
            builder.RegisterType<RedisLockService>().As<ILockService>().InstancePerLifetimeScope(); 
            builder.RegisterType<ValidationService>().As<IValidationService>().InstancePerLifetimeScope();  
            
            base.Load(builder);
        }
    }
}