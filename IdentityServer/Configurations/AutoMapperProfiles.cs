using AutoMapper;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using IdentityServer4.Models;

namespace IdentityServer.Configuration
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<SmsDto, ApplicationUser>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(
                        src => src.PhoneNumber))
                .ForMember(dest => dest.SecurityStamp,
                    opt => opt.MapFrom(
                        src => new Secret("secret", null).Value + src.PhoneNumber.Sha256()
                    ));
        }
    }
}