using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class UpdateLotteryRecordCommandHandler
        : 
          INotificationHandler<UpdateLotteryRecordCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateLotteryRecordCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(
            UpdateLotteryRecordCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                           .CreateActorProxy<ILotteryActor>(
                               new ActorId(
                                   command.Id.ToString()
                               ),
                               nameof(LotteryActor)
                           )
                           .UpdateLotteryRecord(command);
    }
}
