using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
