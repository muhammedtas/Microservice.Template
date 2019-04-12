using System.Linq;
using AutoMapper;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.WebApi.Dto;
using IntradayDashboard.WebApi.Model;
using IntradayDashboard.WebApi.Model.MQModels.Consumption;
using IntradayDashboard.WebApi.Model.MQModels.Offer;

namespace IntradayDashboard.WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;
            CreateMissingTypeMaps = true;
            //CreateMap<User, UserModel>().ForMember(dest => dest.UserRoles.FirstOrDefault(), opt =>
            //{
            //    opt.MapFrom(src => src.UserRoles.FirstOrDefault().ToString());
            //});
            CreateMap<UserRegisterDto, User>(MemberList.None);
            CreateMap<User, UserDto>();
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<UserRole, UserRoleDto>();

            CreateMap<Offer, OfferModel>();
            CreateMap<Consumption, ConsumptionModel>();
        
            CreateMap<ConsumptionDone, Consumption>(MemberList.None);
            CreateMap<OfferDone, Offer>(MemberList.None);
        }
        
    }
}