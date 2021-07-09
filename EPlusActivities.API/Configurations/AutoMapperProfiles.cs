using System.Collections.Specialized;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using IdentityServer4.Models;

namespace EPlusActivities.API.Configuration
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<SmsDto, ApplicationUser>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(
                        src => src.PhoneNumber
                    ))
                .ForMember(dest => dest.NormalizedUserName,
                    opt => opt.MapFrom(
                        src => src.PhoneNumber
                    ));
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();
            CreateMap<LotteryDto, Lottery>();
            CreateMap<Lottery, LotteryDto>();
            CreateMap<AttendanceDto, Attendance>();
            CreateMap<Attendance, AttendanceDto>();
            CreateMap<Activity, ActivityDto>();
        }
    }
}