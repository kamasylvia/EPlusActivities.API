using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForJoinAllAvailableActivitiesDto
    {
        [Required]
        public Guid? UserId { get; set; }
        public IEnumerable<Guid> ActivityIds { get; set; }
    }
}
