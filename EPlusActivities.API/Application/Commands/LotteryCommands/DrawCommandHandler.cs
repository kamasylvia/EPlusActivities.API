using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryActors;
using EPlusActivities.API.Dtos.LotteryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class DrawCommandHandler
        :
          IRequestHandler<DrawCommand, IEnumerable<LotteryDto>>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DrawCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<IEnumerable<LotteryDto>> Handle(
            DrawCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                           .CreateActorProxy<ILotteryActor>(
                               new ActorId(
                                   command.UserId.ToString() + command.ActivityId.ToString()
                               ),
                               nameof(LotteryActor)
                           )
                           .Draw(command);
    }
}
