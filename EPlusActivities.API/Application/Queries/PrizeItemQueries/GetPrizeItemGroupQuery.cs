using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeItemQueries
{
    public class GetPrizeItemGroupQuery : IRequest<IEnumerable<PrizeItemDto>>
    {
        /// <summary>
        /// 奖品 ID 列表
        /// </summary>
        /// <value></value>
        [Required]
        public string Ids { get; set; }
    }
}
