using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.BrandQueries
{
    public class GetBrandListQuery : DtoForGetList, IRequest<IEnumerable<BrandDto>>
    {
    }
}
