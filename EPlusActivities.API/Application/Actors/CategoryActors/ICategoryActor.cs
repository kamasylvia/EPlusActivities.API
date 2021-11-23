using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.CategoryCommands;
using EPlusActivities.API.Dtos.CategoryDtos;

namespace EPlusActivities.API.Application.Actors.CategoryActors
{
    public interface ICategoryActor : IActor
    {
        Task<CategoryDto> CreateCategory(CreateCategoryCommand command);
        Task DeleteCategory(DeleteCategoryCommand command);
        Task UpdateCategory(UpdateCategoryCommand command);
    }
}
