using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeItemQueries
{
    public class GetPrizeItemListQuery : DtoForGetList, IRequest<IEnumerable<PrizeItemDto>>
    {
    }
}
