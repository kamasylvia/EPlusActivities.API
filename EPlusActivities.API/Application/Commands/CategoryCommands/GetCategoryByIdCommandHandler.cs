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

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class GetCategoryByIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetCategoryByIdCommand, CategoryDto>
    {
        public GetCategoryByIdCommandHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<CategoryDto> Handle(
            GetCategoryByIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var category = await _categoryRepository.FindByIdAsync(request.Id.Value);
            if (category is null)
            {
                throw new NotFoundException("Could not find the category.");
            }
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
