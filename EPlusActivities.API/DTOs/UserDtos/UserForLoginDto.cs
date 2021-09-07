using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForLoginDto
    {
        /// <summary>
        /// 用户 Id
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
