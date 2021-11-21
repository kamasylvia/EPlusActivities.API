using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeItemQueries
{
    public class GetPrizeItemByNameQuery : IRequest<IEnumerable<PrizeItemDto>>
    {
        [Required]
        public string Name { get; set; }
    }
}
