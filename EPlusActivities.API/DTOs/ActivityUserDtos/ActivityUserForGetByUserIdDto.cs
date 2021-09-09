using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [Required]
        public string AvailableChannel { get; set; }
    }
}
