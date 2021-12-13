using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Application.Queries.UserQueries;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Application.Actors.UserActors
{
    public interface IUserActor : IActor
    {
        Task<UserDto> Login(LoginQuery request);

        Task CreateAdminOrManager(CreateAdminOrManagerCommand command);

        Task DeleteUser(DeleteUserCommand command);

        Task UpdatePhone(UpdatePhoneCommand command);

        Task UpdateAsync(ApplicationUser user);
    }
}
