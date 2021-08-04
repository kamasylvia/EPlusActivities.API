using System;
using System.Linq;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.DTOs.ActivityDtos;
using EPlusActivities.API.DTOs.ActivityUserDtos;
using EPlusActivities.API.DTOs.AddressDtos;
using EPlusActivities.API.DTOs.AttendanceDtos;
using EPlusActivities.API.DTOs.BrandDtos;
using EPlusActivities.API.DTOs.CategoryDtos;
using EPlusActivities.API.DTOs.LotteryDtos;
using EPlusActivities.API.DTOs.MemberDtos;
using EPlusActivities.API.DTOs.PrizeItemDtos;
using EPlusActivities.API.DTOs.PrizeTierDtos;
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
            CreateMap<
                SmsDto,
                ApplicationUser
            >()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(
                    dest => dest.NormalizedUserName,
                    opt => opt.MapFrom(src => src.PhoneNumber)
                );

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
            CreateMap<
                AttendanceForAttendDto,
                Attendance
            >();
            CreateMap<Attendance, AttendanceDto>();
            #endregion



            #region Activity
            CreateMap<Activity, ActivityDto>();
            CreateMap<ActivityForCreateDto, Activity>();
            CreateMap<ActivityForUpdateDto, Activity>();
            #endregion

            #region ActivityUser
            CreateMap<
                ActivityUser,
                ActivityUserDto
            >();
            CreateMap<ActivityUser, ActivityUserForRedeemDrawsResponseDto>();
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
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<PrizeItemForCreateDto, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<PrizeItemForUpdateDto, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            #endregion



            #region PrizeTier
            CreateMap<PrizeTier, PrizeTierDto>()
                .ForMember(
                    dest => dest.PrizeItemIds,
                    opt => opt.MapFrom(src => src.PrizeTierPrizeItems.Select(x => x.PrizeItem.Id))
                );
            CreateMap<PrizeTierForCreateDto, PrizeTier>()
                .ForMember(dest => dest.Activity, opt => opt.Ignore());
            CreateMap<PrizeTierForUpdateDto, PrizeTier>()
                .ForMember(dest => dest.Activity, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            #endregion



            #region Credit
            CreateMap<
                MemberForUpdateCreditRequestDto,
                Credit
            >();
            CreateMap<MemberForUpdateCreditResponseDto, Credit>()
                .ForMember(
                    dest => dest.MemberId,
                    opt => opt.MapFrom(src => src.Body.Content.MemberId)
                )
                .ForMember(
                    dest => dest.NewPoints,
                    opt => opt.MapFrom(src => src.Body.Content.NewPoints)
                )
                .ForMember(
                    dest => dest.OldPoints,
                    opt => opt.MapFrom(src => src.Body.Content.OldPoints)
                )
                .ForMember(
                    dest => dest.RecordId,
                    opt => opt.MapFrom(src => src.Body.Content.RecordId)
                );
            #endregion


            #endregion
        }
    }
}
