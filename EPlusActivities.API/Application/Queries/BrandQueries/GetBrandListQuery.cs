using System.Collections.Generic;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.BrandQueries
{
    public class GetBrandListQuery : DtoForGetList, IRequest<IEnumerable<BrandDto>>
    {
    }
}
