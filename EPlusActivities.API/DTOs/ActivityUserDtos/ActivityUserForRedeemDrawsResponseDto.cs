using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.ActivityUserDtos
{
    public class ActivityUserForRedeemDrawsResponseDto
    {
        
        [Required]
        public Guid? ActivityId { get; set; }

        [Required]
        public Guid? UserId { get; set; }
    }
}
