using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class DeleteCategoryCommandHandler
        : CategoryRequestHandlerBase,
          IRequestHandler<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(categoryRepository, mapper) { }

        public async Task<Unit> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (!await _categoryRepository.ExistsAsync(request.Id.Value))
            {
                throw new BadRequestException($"Could not find the category.");
            }
            #endregion

            #region Database operations
            var category = await _categoryRepository.FindByIdAsync(request.Id.Value);
            _categoryRepository.Remove(category);
            if (!await _categoryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
