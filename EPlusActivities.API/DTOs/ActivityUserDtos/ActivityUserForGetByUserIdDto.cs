using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public IEnumerable<ChannelCode> AvailableChannels { get; set; }
    }
}
