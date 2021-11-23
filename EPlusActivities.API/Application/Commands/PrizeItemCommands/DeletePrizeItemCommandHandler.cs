using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.PrizeItemActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class DeletePrizeItemCommandHandler
        :
          IRequestHandler<DeletePrizeItemCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeletePrizeItemCommandHandler(
            IActorProxyFactory actorProxyFactory
)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeletePrizeItemCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
          .CreateActorProxy<IPrizeItemActor>(
              new ActorId(
                  command.Id.ToString()
              ),
              nameof(PrizeItemActor)
          ).DeletePrizeItem(command);
            return Unit.Value;
        }
    }
}
