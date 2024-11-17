using AutoMapper;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.ViewModels;

namespace JwtCookiesScheme.Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
