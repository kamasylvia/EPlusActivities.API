using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class GetCategoryListCommand : DtoForGetList, IRequest<IEnumerable<CategoryDto>>
    {
    }
}
