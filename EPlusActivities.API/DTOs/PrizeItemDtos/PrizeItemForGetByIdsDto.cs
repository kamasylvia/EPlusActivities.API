using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeItemDtos
{
    public class PrizeItemForGetByIdsDto
    {
        /// <summary>
        /// 奖品 ID 列表
        /// </summary>
        /// <value></value>
        [Required]
        public string Ids { get; set; }
    }
}
