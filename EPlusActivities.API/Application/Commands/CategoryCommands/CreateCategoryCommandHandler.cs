using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.CategoryActors;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateCategoryCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<CategoryDto> Handle(
            CreateCategoryCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<ICategoryActor>(
                    new ActorId("CreateCategory"),
                    nameof(CategoryActor)
                )
                .CreateCategory(command);
    }
}
