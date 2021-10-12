using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.FileDtos
{
    public class DownloadFilesByOwnerIdRequestDto
    {
        /// <summary>
        /// 文件拥有者的 ID，为保持全局唯一性，使用 Guid
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? OwnerId { get; set; }
    }
}
