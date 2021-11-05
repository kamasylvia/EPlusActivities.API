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
        public string Key { get; set; }

        /// <summary>
        /// 使用 Http 协议发送的文件
        /// </summary>
        /// <value></value>
        [Required]
        public IFormFile FormFile { get; set; }
    }

    public record UploadFileResponse
    {
        public bool Succeeded { get; set; }
    }
}
