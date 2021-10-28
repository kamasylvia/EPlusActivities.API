using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class GetPrizeItemGroupCommand : IRequest<IEnumerable<PrizeItemDto>>
    {
        /// <summary>
        /// 奖品 ID 列表
        /// </summary>
        /// <value></value>
        [Required]
        public string Ids { get; set; }
    }
}
