using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.UserCommands;

namespace EPlusActivities.API.Application.Actors.UserActors
{
    public interface IUserActor : IActor
    {
        Task CreateAdminOrManager(CreateAdminOrManagerCommand command);

        Task DeleteUser(DeleteUserCommand command);

        Task UpdatePhone(UpdatePhoneCommand command);
    }
}
