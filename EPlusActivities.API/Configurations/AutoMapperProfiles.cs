using System;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.DTOs.ActivityDtos;
using EPlusActivities.API.DTOs.AddressDtos;
using EPlusActivities.API.DTOs.AttendanceDtos;
using EPlusActivities.API.DTOs.BrandDtos;
using EPlusActivities.API.DTOs.CategoryDtos;
using EPlusActivities.API.DTOs.LotteryDtos;
using EPlusActivities.API.DTOs.PrizeItemDtos;
using EPlusActivities.API.DTOs.PrizeTypeDtos;
using EPlusActivities.API.DTOs.UserDtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Configuration
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Global configurations.
            CreateMap<string, string>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? null : s.Trim());
            CreateMap<DateTime, DateTime>().ConvertUsing(d => d.Date);
            #endregion



            #region Dtos to Entities and vice versa.
            CreateMap<SmsDto, ApplicationUser>()
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.NormalizedUserName,
                opt => opt.MapFrom(src => src.PhoneNumber));


            #region ApplicationUser
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.RegisterChannel, opt => opt.Ignore())
                .ForMember(dest => dest.LoginChannel, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore());
            #endregion



            #region Address
            CreateMap<Address, AddressDto>();
            CreateMap<AddressForCreateDto, Address>();
            CreateMap<AddressForUpdateDto, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            #endregion



            #region Lottery
            CreateMap<Lottery, LotteryDto>();
            CreateMap<LotteryForCreateDto, Lottery>();
            CreateMap<LotteryForUpdateDto, Lottery>();
            #endregion



            #region Attendance
            CreateMap<Attendance, AttendanceDto>().ReverseMap();
            #endregion



            #region Activity
            CreateMap<Activity, ActivityDto>();
            CreateMap<ActivityForCreateDto, Activity>();
            CreateMap<ActivityForUpdateDto, Activity>();
            #endregion



            #region Brand
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<BrandForGetByNameDto, Brand>();
            #endregion



            #region Category
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryForGetByNameDto, Category>();
            #endregion



            #region PrizeItem
            CreateMap<PrizeItem, PrizeItemDto>()
                .ForMember(dest => dest.BrandName,
                opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<PrizeItemForCreateDto, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<PrizeItemForUpdateDto, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            #endregion



            #region PrizeType
            CreateMap<PrizeType, PrizeTypeDto>();
            CreateMap<PrizeTypeForCreateDto, PrizeType>()
                .ForMember(dest => dest.Activity, opt => opt.Ignore());
            CreateMap<PrizeTypeForUpdateDto, PrizeType>()
                .ForMember(dest => dest.Activity, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            #endregion


            #endregion
        }
    }
}
