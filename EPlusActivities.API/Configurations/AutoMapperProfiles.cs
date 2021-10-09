using System;
using System.Linq;
using AutoMapper;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Dtos.AddressDtos;
using EPlusActivities.API.Dtos.AttendanceDtos;
using EPlusActivities.API.Dtos.BrandDtos;
using EPlusActivities.API.Dtos.CategoryDtos;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Configuration
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Global configurations.
            CreateMap<string, string>()
                .ConvertUsing(s => string.
                IsNullOrEmpty(s) ?
                 null :
                  s.Trim());
            CreateMap<DateTime, DateTime>().ConvertUsing(d => d.Date);
            #endregion



            #region Dtos to Entities and vice versa.
            CreateMap<SmsDto, ApplicationUser>()
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
            CreateMap<Lottery, LotteryDto>()
                .ForMember(
                    dest => dest.ChannelCode,
                    opt => opt.MapFrom(src => src.ChannelCode.ToString())
                )
                .ForMember(
                    dest => dest.LotteryDisplay,
                    opt => opt.MapFrom(src => src.LotteryDisplay.ToString())
                );
            CreateMap<Lottery, LotteryForGetByActivityCodeResponse>()
                .ForMember(dest => dest.DateTime, opt => opt.Ignore())
                .ForMember(
                    dest => dest.ChannelCode,
                    opt => opt.MapFrom(src => src.ChannelCode.ToString())
                )
                .ForMember(
                    dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User.PhoneNumber)
                )
                .ForMember(
                    dest => dest.ActivityCode,
                    opt => opt.MapFrom(src => src.Activity.ActivityCode)
                )
                .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity.Name))
                .ForMember(
                    dest => dest.PrizeTierName,
                    opt => opt.MapFrom(src => src.PrizeTier.Name)
                )
                .ForMember(
                    dest => dest.PrizeType,
                    opt => opt.MapFrom(src => src.PrizeItem.PrizeType.ToString())
                );
            CreateMap<LotteryForCreateDto, Lottery>()
                .ForMember(
                    dest => dest.ChannelCode,
                    opt => opt.MapFrom(src => Enum.Parse<ChannelCode>(src.ChannelCode, true))
                )
                .ForMember(
                    dest => dest.LotteryDisplay,
                    opt => opt.MapFrom(src => Enum.Parse<LotteryDisplay>(src.LotteryDisplay, true))
                );
            CreateMap<LotteryForUpdateDto, Lottery>();
            #endregion



            #region Attendance
            CreateMap<AttendanceForAttendDto, Attendance>()
                .ForMember(
                    dest => dest.ChannelCode,
                    opt => opt.MapFrom(src => Enum.Parse<ChannelCode>(src.ChannelCode, true))
                );
            CreateMap<Attendance, AttendanceDto>()
                .ForMember(
                    dest => dest.ChannelCode,
                    opt => opt.MapFrom(src => src.ChannelCode.ToString())
                );
            #endregion



            #region Activity
            CreateMap<Activity, ActivityDto>()
                .ForMember(
                    dest => dest.AvailableChannels,
                    opt =>
                        opt.MapFrom(
                            src => src.AvailableChannels.Select(channel => channel.ToString())
                        )
                )
                .ForMember(
                    dest => dest.LotteryDisplay,
                    opt => opt.MapFrom(src => src.LotteryDisplay.ToString())
                )
                .ForMember(
                    dest => dest.ActivityType,
                    opt => opt.MapFrom(src => src.ActivityType.ToString())
                );
            CreateMap<ActivityForCreateDto, Activity>()
                .ForMember(
                    dest => dest.AvailableChannels,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.AvailableChannels.Select(
                                    channelString => Enum.Parse<ChannelCode>(channelString, true)
                                )
                        )
                )
                .ForMember(
                    dest => dest.LotteryDisplay,
                    opt => opt.MapFrom(src => Enum.Parse<LotteryDisplay>(src.LotteryDisplay, true))
                )
                .ForMember(
                    dest => dest.ActivityType,
                    opt => opt.MapFrom(src => Enum.Parse<ActivityType>(src.ActivityType, true))
                );
            CreateMap<ActivityForUpdateDto, Activity>()
                .ForMember(
                    dest => dest.LotteryDisplay,
                    opt => opt.MapFrom(src => Enum.Parse<LotteryDisplay>(src.LotteryDisplay, true))
                );
            #endregion



            #region ActivityUser
            CreateMap<ActivityUser, ActivityUserDto>();
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
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(
                    dest => dest.PrizeType,
                    opt => opt.MapFrom(src => src.PrizeType.ToString())
                );
            CreateMap<PrizeItemForCreateDto, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrizeType,
                    opt => opt.MapFrom(src => Enum.Parse<PrizeType>(src.PrizeType, true))
                );
            CreateMap<PrizeItemForUpdateDto, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(
                    dest => dest.PrizeType,
                    opt => opt.MapFrom(src => Enum.Parse<PrizeType>(src.PrizeType, true))
                );
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
            CreateMap<MemberForUpdateCreditRequestDto, Credit>();
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
