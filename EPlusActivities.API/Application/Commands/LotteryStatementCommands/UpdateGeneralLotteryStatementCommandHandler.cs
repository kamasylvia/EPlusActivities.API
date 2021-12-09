using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryStatementActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryStatementCommands
{
    public class UpdateGeneralLotteryStatementCommandHandler
        : INotificationHandler<UpdateGeneralLotteryStatementCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateGeneralLotteryStatementCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(
            UpdateGeneralLotteryStatementCommand notification,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<ILotteryStatementActor>(
                    new ActorId(
                        notification.ActivityId
                            + notification.Channel.ToString()
                            + notification.DateTime.ToOADate()
                    ),
                    nameof(LotteryStatementActor)
                )
                .UpdateGeneralLotteryStatementAsync(notification);
        }
    }
}
