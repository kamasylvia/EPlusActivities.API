using System.IO;
using AutoMapper;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Dtos.FileDtos;
using FileService.Entities;

namespace FileService.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UploadFileRequestDto, AppFile>()
                .ForMember(
                    dest => dest.ContentType,
                    opt => opt.MapFrom(src => src.FormFile.ContentType)
                );
            CreateMap<UploadFileGrpcRequest, AppFile>();
            CreateMap<AppFile, GetFileByOwnerIdResponseDto>();
        }
    }
}
