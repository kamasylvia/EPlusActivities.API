using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.CategoryDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class GetCategoryByContainedNameCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetCategoryByContainedNameCommand, CategoryDto>
    {
        public GetCategoryByContainedNameCommandHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<CategoryDto> Handle(
            GetCategoryByContainedNameCommand request,
            CancellationToken cancellationToken
        )
        {
            var category = await _categoryRepository.FindByContainedNameAsync(request.Keyword);
            if (category is null)
            {
                throw new NotFoundException("Could not find the category.");
            }
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
