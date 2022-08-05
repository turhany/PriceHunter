using AutoMapper;
using PriceHunter.Contract.App.Product;
using PriceHunter.Contract.Service.Product;

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<CreateProductRequest, CreateProductRequestServiceRequest>();
            CreateMap<UpdateProductRequest, UpdateProductRequestServiceRequest>();
        }
    }
}
