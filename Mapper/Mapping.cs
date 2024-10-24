using AutoMapper;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.ViewModels;

namespace JwtCookiesScheme.Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole!.RoleName))
                .ForMember(dest => dest.UserPermissions, opt => opt.MapFrom(src => src.UserRole!.RolePermissions!.Select(rp => rp.Permission!.PermissionName).ToList()));
        }
    }
}
