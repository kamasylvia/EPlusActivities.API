using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Actors;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class JoinAvailableActivitiesCommandHandler
        : ActivityUserRequestHandlerBase,
          INotificationHandler<UserCommands.LoginCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public JoinAvailableActivitiesCommandHandler(
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

        public async Task Handle(
            UserCommands.LoginCommand notification,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(notification.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            await _actorProxyFactory
                .CreateActorProxy<IActivityUserActor>(
                    new ActorId(
                        notification.UserId.ToString() + notification.ChannelCode.ToString()
                    ),
                    nameof(ActivityUserActor)
                )
                .BindUserWithAvailableActivitiesAsync(notification);
        }
    }
}
