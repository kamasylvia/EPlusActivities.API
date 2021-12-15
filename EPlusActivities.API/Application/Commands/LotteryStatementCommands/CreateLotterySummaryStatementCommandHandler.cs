using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryStatementActors;
using EPlusActivities.API.Extensions;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryStatementCommands
{
    public class CreateLotterySummaryStatementCommandHandler
        : INotificationHandler<CreateLotterySummaryStatementCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateLotterySummaryStatementCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(
            CreateLotterySummaryStatementCommand notification,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<ILotteryStatementActor>(
                    new ActorId(
                        notification.ActivityId
                            + notification.Channel.ToString()
                            + notification.Date.ToOADate()
                    ),
                    nameof(LotteryStatementActor)
                )
                .CreateLotterySummaryStatementAsync(notification);
    }
}
