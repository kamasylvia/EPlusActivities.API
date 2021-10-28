using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class CreatePrizeTierCommand : IRequest<PrizeTierDto>
    {
        /// <summary>
        /// 奖品档次名称，如：一等奖、二等奖、安慰奖
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 获奖概率
        /// </summary>
        /// <value></value>
        public int Percentage { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public IEnumerable<Guid> PrizeItemIds { get; set; }

        /// <summary>
        /// 每日中奖上限  
        /// </summary>
        /// <value></value>
        public int? DailyLimit { get; set; }
    }
}
