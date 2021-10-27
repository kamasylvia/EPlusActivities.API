using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityUserDtos;
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
    public class BindActivityAndUserCommandHandler
        : BaseCommandHandler,
          IRequestHandler<BindActivityAndUserCommand>
    {
        public BindActivityAndUserCommandHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
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

        public async Task<Unit> Handle(
            BindActivityAndUserCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.Value.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                request.ActivityId.Value,
                request.UserId.Value
            );
            if (activityUser is not null)
            {
                throw new ConflictException("The user had already joined the activity.");
            }
            #endregion

            #region Create an ActivityUser link
            activityUser = new ActivityUser { Activity = activity, User = user, };
            #endregion

            #region Database operations
            await _activityUserRepository.AddAsync(activityUser);
            if (!await _activityUserRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return new Unit();
        }
    }
}
