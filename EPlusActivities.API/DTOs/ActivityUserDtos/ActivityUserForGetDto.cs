using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForGetDto
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
