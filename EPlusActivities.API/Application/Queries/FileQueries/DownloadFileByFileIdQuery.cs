using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.FileQueries
{
    public class DownloadFileByFileIdQuery : IRequest<DownloadFileGrpcResponse>
    {
        /// <summary>
        /// 文件 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? FileId { get; set; }
    }
}
