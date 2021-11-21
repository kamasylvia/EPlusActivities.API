using System;
using AutoMapper;
using EPlusActivities.API.Services.FileService;

namespace EPlusActivities.API.Application
{
    public class FileRequestHandlerBase
    {
        protected readonly IMapper _mapper;
        protected readonly IFileService _fileService;

        public FileRequestHandlerBase(IMapper mapper, IFileService fileService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }
    }
}
