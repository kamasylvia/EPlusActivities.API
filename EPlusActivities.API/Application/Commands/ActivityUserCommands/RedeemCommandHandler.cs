using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
                        command.ActivityId.Value.ToString()
                            + command.UserId.ToString()
                            + command.Channel.ToString()
                    ),
                    nameof(ActivityUserActor)
                )
                .Redeem(command);
    }
}
