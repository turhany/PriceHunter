using AutoMapper;
using PriceHunter.Contract.App.Supplier;
using PriceHunter.Model.Supplier;

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class SupplierMapping : Profile
    {
        public SupplierMapping()
        {
            CreateMap<Supplier, SupplierViewModel>();
        }
    }
}
