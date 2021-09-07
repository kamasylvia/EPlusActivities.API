using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserDto
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        /// <value></value>
        [Required]
        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 是否会员
        /// </summary>
        /// <value></value>
        public bool IsMember { get; set; }

        /// <summary>
        /// 会员 ID
        /// </summary>
        /// <value></value>
        public string MemberId { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        /// <value></value>
        public int Credit { get; set; }
    }
}
