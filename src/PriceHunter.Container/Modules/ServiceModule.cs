using System.Reflection;
using Autofac;
using PriceHunter.Business.Product.Concrete;
using PriceHunter.Common.Data.Abstract;
using Module = Autofac.Module;

namespace PriceHunter.Container.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            TypeInfo assemblyType = typeof(ProductService).GetTypeInfo();

            builder.RegisterAssemblyTypes(assemblyType.Assembly)
                .Where(x => typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}