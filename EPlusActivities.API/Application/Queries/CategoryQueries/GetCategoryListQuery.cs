using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.CategoryQueries
{
    public class GetCategoryListQuery : DtoForGetList, IRequest<IEnumerable<CategoryDto>>
    {
    }
}
