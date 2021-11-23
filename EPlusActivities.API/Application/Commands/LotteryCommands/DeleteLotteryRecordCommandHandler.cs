using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class DeleteLotteryRecordCommandHandler : IRequestHandler<DeleteLotteryRecordCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteLotteryRecordCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        public async Task<Unit> Handle(
            DeleteLotteryRecordCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<ILotteryActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(LotteryActor)
                )
                .DeleteLotteryRecord(command);
            return Unit.Value;
        }
    }
}
