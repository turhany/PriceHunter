using AutoMapper;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Service.Product;
using PriceHunter.Model.Product;

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
            CreateMap<Product, ProductSearchViewModel>();
            CreateMap<ProductSupplierInfoMapping, ProductSupplierInfoMappingViewModel>();
            CreateMap<ProductPriceHistory, ProductPriceHistorySearchViewModel>();
        }
    }
}
