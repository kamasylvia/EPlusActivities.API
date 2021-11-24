using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class RedeemCommandHandler
        : IRequestHandler<RedeemCommand, ActivityUserForRedeemDrawsResponseDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public RedeemCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<ActivityUserForRedeemDrawsResponseDto> Handle(
            RedeemCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IActivityUserActor>(
                    new ActorId(
                        command.ActivityId.ToString()
                            + command.UserId.ToString()
                            + command.Channel.ToString()
                    ),
                    nameof(ActivityUserActor)
                )
                .Redeem(command);
    }
}
