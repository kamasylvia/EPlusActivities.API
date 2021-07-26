using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.UserDtos
{
    public class UserForLoginDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public ChannelCode? LoginChannel { get; set; }
    }
}
