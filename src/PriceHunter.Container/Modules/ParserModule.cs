using Autofac;
using PriceHunter.Model.Supplier;
using PriceHunter.Parser;
using PriceHunter.Parser.Alibaba;
using PriceHunter.Parser.AliExpress;
using PriceHunter.Parser.Amazon; 

namespace PriceHunter.Container.Modules
{
    public class ParserModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AmazonParser>().Keyed<IParser>(SupplierType.Amazon).InstancePerLifetimeScope();
            builder.RegisterType<AlibabaParser>().Keyed<IParser>(SupplierType.Alibaba).InstancePerLifetimeScope();
            builder.RegisterType<AliExpressParser>().Keyed<IParser>(SupplierType.AliExpress).InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
