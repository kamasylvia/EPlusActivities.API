using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public record UploadFileCommand : IRequest
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
        [RegularExpression("[ -~]*")]
        public string Key { get; set; }

        /// <summary>
        /// 是否为静态文件。静态文件返回 URL，非静态文件返回文件流以供下载。
        /// </summary>
        /// <value></value>
        [Required]
        public bool IsStatic { get; set; }

        /// <summary>
        /// 使用 Http 协议发送的文件
        /// </summary>
        /// <value></value>
        [Required]
        public IFormFile FormFile { get; set; }
    }
}
