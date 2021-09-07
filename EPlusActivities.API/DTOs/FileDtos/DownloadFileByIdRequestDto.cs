using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.FileDtos
{
    public class DownloadFileByIdRequestDto
    {
        /// <summary>
        /// 文件 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? FileId { get; set; }
    }
}
