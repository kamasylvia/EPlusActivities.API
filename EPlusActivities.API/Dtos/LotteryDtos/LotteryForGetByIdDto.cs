using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForGetByIdDto
    {
        /// <summary>
        /// 抽奖记录 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
