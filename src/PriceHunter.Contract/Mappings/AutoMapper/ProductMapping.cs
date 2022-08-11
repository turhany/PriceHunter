using AutoMapper;
using PriceHunter.Common.Extensions;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Service.Product;
using PriceHunter.Model.Product;
using PriceHunter.Model.Supplier;

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<CreateProductRequest, CreateProductRequestServiceRequest>();
            CreateMap<UpdateProductRequest, UpdateProductRequestServiceRequest>();
            CreateMap<ProductSupplierInfoMappingViewModel, ProductSupplierInfoMappingServiceModel>();
            CreateMap<Product, ProductViewModel>(); 
            CreateMap<ProductSupplierInfoMapping, ProductSupplierInfoMappingViewModel>()
                .ForMember(p => p.SupplierType, b => b.MapFrom(
                    p =>
                    Enum.Parse<SupplierType>(HelpersToolbox.Extensions.EnumExtensions.EnumToList<SupplierType>()
                    .First(k => Enum.Parse<SupplierType>(k.Value).GetDatabaseId() == p.SupplierId).Value)
                    ));

        }
    }
}
