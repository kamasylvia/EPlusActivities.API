using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.PrizeItemActors;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class CreatePrizeItemCommandHandler
        :
          IRequestHandler<CreatePrizeItemCommand, PrizeItemDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreatePrizeItemCommandHandler(
            IActorProxyFactory actorProxyFactory
)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }
        public async Task<PrizeItemDto> Handle(
            CreatePrizeItemCommand command,
            CancellationToken cancellationToken
        ) => await _actorProxyFactory
                   .CreateActorProxy<IPrizeItemActor>(
                       new ActorId(
                           command.Name
                       ),
                       nameof(PrizeItemActor)
                   ).CreatePrizeItem(command);
    }
}
