using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.FileDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFileByIdCommand : IRequest<DownloadFileByIdDto>
    {
        /// <summary>
        /// 文件 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? FileId { get; set; }
    }

    public class DownloadFileByIdDto
    {
        public byte[] FileContents { get; set; }
        public string ContentType { get; set; }
    }
}
