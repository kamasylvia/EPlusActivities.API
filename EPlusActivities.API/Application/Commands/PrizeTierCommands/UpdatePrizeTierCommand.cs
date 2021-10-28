using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class UpdatePrizeTierCommand : IRequest
    {
        [Required]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 获奖概率
        /// </summary>
        /// <value></value>
        public int Percentage { get; set; }

        /// <summary>
        /// 每日中奖上限  
        /// </summary>
        /// <value></value>
        public int? DailyLimit { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public IEnumerable<Guid> PrizeItemIds { get; set; }
    

    }
}
