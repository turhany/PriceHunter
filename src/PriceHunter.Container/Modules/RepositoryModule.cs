using System.Reflection;
using Autofac;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Data.MongoDB.Repositories; 
using PriceHunter.Data.Repositories.Abstract;
using Module = Autofac.Module;

namespace PriceHunter.Container.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(MongoDBGenericRepository<>))
                .As(typeof(IGenericRepository<>))
                .InstancePerLifetimeScope(); 
            
            
            TypeInfo assemblyType = typeof(IProductRepository).GetTypeInfo();
            
            builder.RegisterAssemblyTypes(assemblyType.Assembly)
                .Where(x => typeof(IRepository).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}