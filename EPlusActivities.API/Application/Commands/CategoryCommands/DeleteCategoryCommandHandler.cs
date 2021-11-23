using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.CategoryActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        public DeleteCategoryCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteCategoryCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<ICategoryActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(CategoryActor)
                )
                .DeleteCategory(command);
            return Unit.Value;
        }
    }
}
