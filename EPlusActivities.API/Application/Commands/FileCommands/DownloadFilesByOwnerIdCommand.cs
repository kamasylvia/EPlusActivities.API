using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.FileDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFilesByOwnerIdCommand : IRequest<IEnumerable<DownloadFilesByOwnerIdDto>>
    {
        /// <summary>
        /// 文件拥有者的 ID，为保持全局唯一性，使用 Guid
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? OwnerId { get; set; }
    }
}
