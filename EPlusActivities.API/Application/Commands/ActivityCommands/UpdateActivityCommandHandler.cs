using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityActors;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.ActivityCommands
{
    public class UpdateActivityCommandHandler
        :
          IRequestHandler<UpdateActivityCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateActivityCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdateActivityCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                     .CreateActorProxy<IActivityActor>(
                         new ActorId(
                             command.Id.Value.ToString()
                         ),
                         nameof(ActivityActor)
                     ).UpdateActivity(command);
            return Unit.Value;
        }
    }
}
