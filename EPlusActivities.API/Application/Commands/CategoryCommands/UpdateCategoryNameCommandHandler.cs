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
    public class UpdateCategoryNameCommandHandler
        : CategoryRequestHandlerBase,
          IRequestHandler<UpdateCategoryNameCommand>
    {
        public UpdateCategoryNameCommandHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<Unit> Handle(
            UpdateCategoryNameCommand request,
            CancellationToken cancellationToken
        )
        {
            var category = await _categoryRepository.FindByIdAsync(request.Id.Value);

            #region Parameter validation
            if (category is null)
            {
                throw new NotFoundException($"Could not find the category with ID '{request.Id}'");
            }

            if (await _categoryRepository.ExistsAsync(request.Name))
            {
                throw new ConflictException($"The category '{request.Name}' is already existed.");
            }
            #endregion

            #region Database operations
            _categoryRepository.Update(
                _mapper.Map<UpdateCategoryNameCommand, Category>(request, category)
            );
            if (!await _categoryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
