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
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.NormalizedUserName,
                opt => opt.MapFrom(src => src.PhoneNumber));
            CreateMap<ApplicationUser, UserDetailsDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<Lottery, LotteryDto>().ReverseMap();
            CreateMap<Attendance, AttendanceDto>().ReverseMap();
            CreateMap<Activity, ActivityDto>().ReverseMap();
            CreateMap<PrizeItem, PrizeItemDto>()
                .ForMember(dest => dest.BrandName,
                opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<PrizeItemDto, PrizeItem>();
            CreateMap<PrizeType, PrizeTypeDto>().ReverseMap();
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
