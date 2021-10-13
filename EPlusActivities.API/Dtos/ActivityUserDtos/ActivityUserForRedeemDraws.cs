using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public class ActivityUserForRedeemDrawsRequestDto
    {
        [Required]
        public Guid? ActivityId { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 渠道号
        /// </summary>
        /// <value></value>
        [Required]
        public string Channel { get; set; }

        /// <summary>
        /// 兑换几次
        /// </summary>
        /// <value></value>
        public int Count { get; set; }

        /// <summary>
        /// 每次所需积分
        /// </summary>
        /// <value></value>
        public int UnitPrice { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        /// <value></value>
        public string Reason { get; set; }
    }
}
