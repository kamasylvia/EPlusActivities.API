using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DeleteFileByFileIdCommand : IRequest
    {
        /// <summary>
        /// 文件 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? FileId { get; set; }
    }
}
