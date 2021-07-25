using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.UserDtos
{
    public class UserForDeleteDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
