using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.PrizeItemActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class UpdatePrizeItemCommandHandler : IRequestHandler<UpdatePrizeItemCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdatePrizeItemCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }
        public async Task<Unit> Handle(
            UpdatePrizeItemCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IPrizeItemActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(PrizeItemActor)
                )
                .UpdatePrizeItem(command);
            return Unit.Value;
        }
    }
}
