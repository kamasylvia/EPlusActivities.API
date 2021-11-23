using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.BrandActors;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class CreateBrandCommandHandler
        :
          IRequestHandler<CreateBrandCommand, BrandDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateBrandCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<BrandDto> Handle(
            CreateBrandCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IBrandActor>(
                    new ActorId(
                        command.Name
                    ),
                    nameof(BrandActor)
                )
                .CreateBrand(command);
    }
}
