using AutoMapper; 
using PriceHunter.Common.Extensions;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Service.UserProduct;
using PriceHunter.Model.Supplier;
using PriceHunter.Model.UserProduct; 

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class UserProductMapping : Profile
    {
        public UserProductMapping()
        {
            CreateMap<CreateUserProductRequest, CreateUserProductRequestServiceRequest>();
            CreateMap<UpdateUserProductRequest, UpdateUserProductRequestServiceRequest>();
            CreateMap<UrlSupplierMappingViewModel, UrlSupplierMappingServiceModel>();
            CreateMap<UserProduct, UserProductViewModel>();
            CreateMap<UserProductSupplierMapping, UrlSupplierMappingViewModel>()
                .ForMember(p => p.SupplierType, b => b.MapFrom(
                    p =>
                    Enum.Parse<SupplierType>(HelpersToolbox.Extensions.EnumExtensions.EnumToList<SupplierType>()
                    .First(k => Enum.Parse<SupplierType>(k.Value).GetDatabaseId() == p.SupplierId).Value)
                    ));
        }
    }
}
