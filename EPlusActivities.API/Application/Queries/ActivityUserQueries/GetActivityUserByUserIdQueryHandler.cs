using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Actors;
using EPlusActivities.API.Application.Queries.UserQueries;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Queries.ActivityUserQueries
{
    public class GetActivityUserByUserIdQueryHandler
        : ActivityUserRequestHandlerBase,
          IRequestHandler<GetActivityUserByUserIdQuery, IEnumerable<ActivityUserDto>>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public GetActivityUserByUserIdQueryHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository,
            IActorProxyFactory actorProxyFactory
        )
            : base(
                activityRepository,
                memberService,
                userManager,
                activityUserRepository,
                mapper,
                idGeneratorService,
                activityService,
                statementRepository
            )
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<IEnumerable<ActivityUserDto>> Handle(
            GetActivityUserByUserIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var actor = _actorProxyFactory.CreateActorProxy<IActivityUserActor>(
                new ActorId(request.UserId.ToString() + request.AvailableChannel.ToString()),
                nameof(ActivityUserActor)
            );
            return await actor.BindUserWithAvailableActivitiesAsync(
                new LoginQuery { UserId = request.UserId, ChannelCode = request.AvailableChannel }
            )
              ? await actor.GetActivitiesByUserIdAsync(request)
              : new List<ActivityUserDto>();
        }
    }
}
