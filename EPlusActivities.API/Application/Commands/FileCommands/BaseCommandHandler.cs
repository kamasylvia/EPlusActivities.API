using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Services.FileService;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class BaseCommandHandler
    {
        protected readonly IMapper _mapper;
        protected readonly IFileService _fileService;

        public BaseCommandHandler(IMapper mapper, IFileService fileService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }
    }
}
