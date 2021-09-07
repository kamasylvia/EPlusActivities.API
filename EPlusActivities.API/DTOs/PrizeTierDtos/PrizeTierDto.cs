using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeTierDtos
{
    public class PrizeTierDto
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

        public IEnumerable<Guid> PrizeItemIds { get; set; }
    }
}
