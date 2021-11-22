using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeTierDtos
{
    public record PrizeTierDto
    {
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// 奖品档次名称，如：一等奖、二等奖、安慰奖
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 中奖概率
        /// </summary>
        /// <value></value>
        public int Percentage { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        /// <summary>
        /// 每日中奖上限  
        /// </summary>
        /// <value></value>
        public int? DailyLimit { get; set; }

        /// <summary>
        /// 今日中奖人数
        /// </summary>
        /// <value></value>
        public int TodayWinnerCount { get; set; }

        public IEnumerable<Guid> PrizeItemIds { get; set; }
    }
}
