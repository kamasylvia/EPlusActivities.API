using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeTierDtos
{
    public class PrizeTierForDeletePrizeItemDto
    {
        /// <summary>
        /// 奖品档次 ID 
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 奖品 ID 列表
        /// </summary>
        /// <value></value>
        [Required]
        public IEnumerable<Guid> PrizeItemIds { get; set; }
    }
}
