using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryStatementActors;
using EPlusActivities.API.Extensions;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryStatementCommands
{
    public class UpdateLotterySummaryStatementCommandHandler
        : INotificationHandler<UpdateLotterySummaryStatementCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateLotterySummaryStatementCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(
            UpdateLotterySummaryStatementCommand notification,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<ILotteryStatementActor>(
                    new ActorId(
                        notification.ActivityId
                            + notification.Channel.ToString()
                            + notification.Date.ToOADate()
                    ),
                    nameof(LotteryStatementActor)
                )
                .UpdateLotterySummaryStatementAsync(notification);
        }
    }
}
