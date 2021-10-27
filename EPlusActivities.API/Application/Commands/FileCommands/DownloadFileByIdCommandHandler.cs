using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFileByIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByIdCommand, DownloadFileByIdDto>
    {
        public DownloadFileByIdCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileByIdDto> Handle(
            DownloadFileByIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var fileContents = await _fileService.DownloadFileByIdAsync(request);
            if (fileContents.Length == 0)
            {
                throw new NotFoundException("Could not find the file.");
            }
            var contentType = await _fileService.GetContentTypeByIdAsync(request);

            return new DownloadFileByIdDto
            {
                FileContents = fileContents,
                ContentType = contentType
            };
        }
    }
}
