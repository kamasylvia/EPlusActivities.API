using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForRedeemDrawsResponseDto
    {
        [Required]
        public Guid? ActivityId { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 剩余抽奖次数
        /// </summary>
        /// <value></value>
        public int? RemainingDraws { get; set; }
    }
}
