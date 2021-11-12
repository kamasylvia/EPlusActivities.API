using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityUserDtos;
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
    public class JoinAvailableActivitiesCommandHandler
        : BaseCommandHandler,
          IRequestHandler<JoinAvailableActivitiesCommand, IEnumerable<ActivityUserDto>>,
          INotificationHandler<UserCommands.LoginCommand>
    {
        public JoinAvailableActivitiesCommandHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
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
            ) { }

        public async Task<IEnumerable<ActivityUserDto>> Handle(
            JoinAvailableActivitiesCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            var newCreatedLinks = await _activityService.BindUserWithAvailableActivities(
                request.UserId.Value,
                request.AvailableChannel
            // Enum.Parse<ChannelCode>(request.AvailableChannel, true)
            );

            return _mapper.Map<IEnumerable<ActivityUserDto>>(newCreatedLinks);
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

            var newCreatedLinks = await _activityService.BindUserWithAvailableActivities(
                notification.UserId.Value,
                notification.ChannelCode
            );
        }
    }
}
