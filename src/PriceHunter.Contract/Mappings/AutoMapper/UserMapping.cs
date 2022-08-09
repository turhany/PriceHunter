using AutoMapper;
using HelpersToolbox.Extensions;
using PriceHunter.Contract.App.User;
using PriceHunter.Contract.Service.User;
using PriceHunter.Model.User;

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<CreateUserRequest, CreateUserRequestServiceRequest>();
            CreateMap<UpdateUserRequest, UpdateUserRequestServiceRequest>();
            CreateMap<RefreshTokenContract, RefreshTokenContractServiceRequest>();
            CreateMap<GetTokenContract, GetTokenContractServiceRequest>();
            CreateMap<User, UserViewModel>().ForMember(p => p.Type, b => b.MapFrom(p => p.Type.GetDisplayName()));
        }
    }
}
