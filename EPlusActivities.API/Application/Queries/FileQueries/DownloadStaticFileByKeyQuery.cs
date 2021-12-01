using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.FileQueries
{
    public class DownloadStaticFileByKeyQuery:IRequest<DownloadStaticFileGrpcResponse>
    {
        /// <summary>
        /// 文件拥有者的 ID，为保持唯一性，使用 Guid
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// 文件关键字，与 OwnerId 搭配可确定需要查找的文件
        /// </summary>
        /// <value></value>
        [Required]
        public string Key { get; set; }
    }
}
