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
    public class CreateCategoryCommandHandler
        : CategoryRequestHandlerBase,
          IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        public CreateCategoryCommandHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<CategoryDto> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (await _categoryRepository.ExistsAsync(request.Name))
            {
                throw new ConflictException($"The category '{request.Name}' is already existed.");
            }
            #endregion

            #region New an entity
            var category = _mapper.Map<Category>(request);
            #endregion

            #region Database operations
            await _categoryRepository.AddAsync(category);
            if (!await _categoryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<CategoryDto>(category);
        }
    }
}
