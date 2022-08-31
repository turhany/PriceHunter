using AutoMapper;
using PriceHunter.Contract.App.Currency;
using PriceHunter.Model.Currency; 

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class CurrencyMapping : Profile
    {
        public CurrencyMapping()
        {
            CreateMap<Currency, CurrencyViewModel>();
        }
    }
}
