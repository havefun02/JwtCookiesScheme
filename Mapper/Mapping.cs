using AutoMapper;
using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.ViewModels;

namespace JwtCookiesScheme.Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.PhoneNumber));
            CreateMap<User, UserItemViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRoles.Select(r => r.Role.Name).ToList()));
            CreateMap<User, EditRequest>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles.Select(r => r.Role.Name).ToList()));
        }
    }
}
