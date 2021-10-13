using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForDeleteDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
