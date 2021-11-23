using System;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.CategoryCommands;
using EPlusActivities.API.Dtos.CategoryDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Application.Actors.CategoryActors
{
    public class CategoryActor : Actor, ICategoryActor
    {
        private readonly INameExistsRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryActor(ActorHost host,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(host)
        {
            _categoryRepository =
                categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CategoryDto> CreateCategory(CreateCategoryCommand command)
        {
            #region Parameter validation
            if (await _categoryRepository.ExistsAsync(command.Name))
            {
                throw new ConflictException($"The category '{command.Name}' is already existed.");
            }
            #endregion

            #region New an entity
            var category = _mapper.Map<Category>(command);
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

        public async Task DeleteCategory(DeleteCategoryCommand command)
        {
            #region Parameter validation
            if (!await _categoryRepository.ExistsAsync(command.Id.Value))
            {
                throw new BadRequestException($"Could not find the category.");
            }
            #endregion

            #region Database operations
            var category = await _categoryRepository.FindByIdAsync(command.Id.Value);
            _categoryRepository.Remove(category);
            if (!await _categoryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task UpdateCategory(UpdateCategoryCommand command)
        {
            var category = await _categoryRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (category is null)
            {
                throw new NotFoundException($"Could not find the category with ID '{command.Id}'");
            }

            if (await _categoryRepository.ExistsAsync(command.Name))
            {
                throw new ConflictException($"The category '{command.Name}' is already existed.");
            }
            #endregion

            #region Database operations
            _categoryRepository.Update(
                _mapper.Map<UpdateCategoryCommand, Category>(command, category)
            );
            if (!await _categoryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
