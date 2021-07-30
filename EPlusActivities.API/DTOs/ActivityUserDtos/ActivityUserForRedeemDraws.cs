using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.ActivityUserDtos
{
    public class ActivityUserForRedeemDrawsRequestDto
    {
        [Required]
        public Guid? ActivityId { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public int Count { get; set; }

        public int UnitPrice { get; set; }

        public string Reason { get; set; }
    }
}
