using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForLoginDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
