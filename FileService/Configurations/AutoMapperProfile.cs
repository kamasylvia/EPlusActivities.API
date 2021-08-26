using System.IO;
using AutoMapper;
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
                )
                .ForMember(
                    dest => dest.FilePath,
                    opt =>
                        opt.MapFrom(
                            src => Path.Combine(Path.GetRandomFileName(), src.FormFile.FileName)
                        )
                );
        }
    }
}
