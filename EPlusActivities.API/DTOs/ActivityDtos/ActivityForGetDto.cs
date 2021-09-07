using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForGetDto
    {
        /// <summary>
        /// 活动 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
