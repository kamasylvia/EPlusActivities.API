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
    public class GetCategoryByNameQueryHandler
        : CategoryRequestHandlerBase,
          IRequestHandler<GetCategoryByNameQuery, CategoryDto>
    {
        public GetCategoryByNameQueryHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<CategoryDto> Handle(
            GetCategoryByNameQuery request,
            CancellationToken cancellationToken
        )
        {
            var category = await _categoryRepository.FindByNameAsync(request.Name);
            if (category is null)
            {
                throw new NotFoundException("Could not find the category.");
            }
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
