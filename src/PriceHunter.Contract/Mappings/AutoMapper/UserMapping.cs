using AutoMapper;
using PriceHunter.Contract.App.User;

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<PriceHunter.Model.User.User, UserViewModel>(); 
        }
    }
}
