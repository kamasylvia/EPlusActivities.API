using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.FileQueries
{
    public class DownloadStaticFileByFileIdQuery : IRequest<DownloadStaticFileGrpcResponse>
    {
        [Required]
        public Guid? FileId { get; set; }
    }
}
