using Autofac;
using PriceHunter.Cache.Abstract;
using PriceHunter.Cache.Concrete;
using PriceHunter.Lock.Abstract;
using PriceHunter.Lock.Concrete;
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