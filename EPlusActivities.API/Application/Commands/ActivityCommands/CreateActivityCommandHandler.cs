using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityActors;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
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
    public class CreateActivityCommandHandler
        :
          IRequestHandler<CreateActivityCommand, ActivityDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateActivityCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory;
        }

        public async Task<ActivityDto> Handle(
            CreateActivityCommand command,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (command.StartTime > command.EndTime)
            {
                throw new BadRequestException("The EndTime could not be less than the StartTime.");
            }
            #endregion

            return await _actorProxyFactory
                     .CreateActorProxy<IActivityActor>(
                         new ActorId(
                             command.Name + command.StartTime.ToString()
                         ),
                         nameof(ActivityActor)
                     ).CreateActivity(command);
        }
    }
}
