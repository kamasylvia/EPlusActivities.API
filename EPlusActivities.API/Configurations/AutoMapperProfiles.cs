using System;
using System.Linq;
using AutoMapper;
using EPlusActivities.API.Application.Commands.ActivityCommands;
using EPlusActivities.API.Application.Commands.AddressCommands;
using EPlusActivities.API.Application.Commands.AttendanceCommands;
using EPlusActivities.API.Application.Commands.BrandCommands;
using EPlusActivities.API.Application.Commands.CategoryCommands;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Application.Commands.LotteryCommands;
using EPlusActivities.API.Application.Commands.PrizeItemCommands;
using EPlusActivities.API.Application.Commands.PrizeTierCommands;
using EPlusActivities.API.Application.Commands.SmsCommands;
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
using EPlusActivities.Grpc.Messages.FileService;
using Google.Protobuf;

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
            CreateMap<GetVerificationCodeCommand, ApplicationUser>()
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
            CreateMap<CreateAddressCommand, Address>();
            CreateMap<UpdateAddressCommand, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            #endregion



            #region Lottery
            CreateMap<Lottery, LotteryDto>();
            CreateMap<Lottery, LotteryRecordsForManagerResponse>()
                .ForMember(dest => dest.DateTime, opt => opt.Ignore())
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
                    opt => opt.MapFrom(src => src.PrizeItem.PrizeType)
                );
            CreateMap<GeneralLotteryRecords, LotteryForGetGeneralRecordsResponse>();
            CreateMap<DrawCommand, Lottery>();
            CreateMap<UpdateLotteryRecordCommand, Lottery>();
            #endregion



            #region Attendance
            CreateMap<AttendCommand, Attendance>();
            CreateMap<Attendance, AttendanceDto>();
            #endregion



            #region Activity
            CreateMap<Activity, ActivityDto>()
                .ForMember(
                    dest => dest.AvailableChannels,
                    opt =>
                        opt.MapFrom(
                            src => src.AvailableChannels.Select(channel => channel.ToString())
                        )
                );
            CreateMap<CreateActivityCommand, Activity>()
                .ForMember(
                    dest => dest.AvailableChannels,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.AvailableChannels.Select(
                                    channelString => Enum.Parse<ChannelCode>(channelString, true)
                                )
                        )
                );
            CreateMap<UpdateActivityCommand, Activity>();
            #endregion



            #region ActivityUser
            CreateMap<ActivityUser, ActivityUserDto>()
                .ForMember(
                    dest => dest.RequiredCreditForRedeeming,
                    opt => opt.MapFrom(src => src.Activity.RequiredCreditForRedeeming)
                );
            CreateMap<ActivityUser, ActivityUserForRedeemDrawsResponseDto>();
            #endregion



            #region Brand
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandCommand, Brand>();
            CreateMap<UpdateBrandNameCommand, Brand>();
            #endregion



            #region Category
            CreateMap<Category, CategoryDto>()
                .ReverseMap();
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryNameCommand, Category>();
            #endregion



            #region PrizeItem
            CreateMap<PrizeItem, PrizeItemDto>();
            CreateMap<CreatePrizeItemCommand, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<UpdatePrizeItemCommand, PrizeItem>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            #endregion



            #region PrizeTier
            CreateMap<PrizeTier, PrizeTierDto>()
                .ForMember(
                    dest => dest.PrizeItemIds,
                    opt => opt.MapFrom(src => src.PrizeTierPrizeItems.Select(x => x.PrizeItem.Id))
                );
            CreateMap<CreatePrizeTierCommand, PrizeTier>()
                .ForMember(dest => dest.Activity, opt => opt.Ignore());
            CreateMap<UpdatePrizeTierCommand, PrizeTier>()
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



            #region File service
            CreateMap<UploadFileCommand, UploadFileGrpcRequest>()
                .ForMember(
                    dest => dest.ContentType,
                    opt => opt.MapFrom(src => src.FormFile.ContentType)
                )
                .ForMember(
                    dest => dest.Content,
                    opt => opt.MapFrom(src => ByteString.FromStream(src.FormFile.OpenReadStream()))
                );
            CreateMap<DownloadFileByKeyCommand, DownloadFileByKeyGrpcRequest>();
            CreateMap<DownloadFileByIdCommand, DownloadFileByIdGrpcRequest>();
            #endregion


            #endregion
        }
    }
}
