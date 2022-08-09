﻿using AutoMapper;
using PriceHunter.Contract.App.UserProduct;
using PriceHunter.Contract.Service.UserProduct; 

namespace PriceHunter.Contract.Mappings.AutoMapper
{
    public class UserProductMapping : Profile
    {
        public UserProductMapping()
        {
            CreateMap<CreateUserProductRequest, CreateUserProductRequestServiceRequest>();
            CreateMap<UpdateUserProductRequest, UpdateUserProductRequestServiceRequest>();
            CreateMap<UrlSupplierMappingViewModel, UrlSupplierMappingServiceModel>();
        }
    }
}