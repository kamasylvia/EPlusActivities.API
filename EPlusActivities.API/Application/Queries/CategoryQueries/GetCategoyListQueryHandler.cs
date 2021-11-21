using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.CategoryDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Queries.CategoryQueries
{
    public class GetCategoyListQueryHandler
        : CategoryRequestHandlerBase,
          IRequestHandler<GetCategoryListQuery, IEnumerable<CategoryDto>>
    {
        public GetCategoyListQueryHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<IEnumerable<CategoryDto>> Handle(
            GetCategoryListQuery request,
            CancellationToken cancellationToken
        )
        {
            var list = (await _categoryRepository.FindAllAsync()).OrderBy(c => c.Name).ToList();

            var startIndex = (request.PageIndex - 1) * request.PageSize;
            var count = request.PageIndex * request.PageSize;

            if (list.Count < startIndex)
            {
                throw new NotFoundException($"Could not find any category.");
            }

            if (list.Count - startIndex < count)
            {
                count = list.Count - startIndex;
            }

            var result = list.GetRange(startIndex, count);
            if (result.Count <= 0)
            {
                throw new NotFoundException($"Could not found any category.");
            }

            return _mapper.Map<IEnumerable<CategoryDto>>(result);
        }
    }
}
