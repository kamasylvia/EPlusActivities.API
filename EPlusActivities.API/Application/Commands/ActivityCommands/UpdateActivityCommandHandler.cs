using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        : BaseCommandHandler,
          IRequestHandler<UpdateActivityCommand>
    {
        public UpdateActivityCommandHandler(
            IMemberService memberService,
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IIdGeneratorService idGeneratorService,
            IActivityUserRepository activityUserRepository,
            ILotteryRepository lotteryRepository,
            IMapper mapper,
            IActivityService activityService,
            ILotteryService lotteryService
        )
            : base(
                memberService,
                activityRepository,
                userManager,
                idGeneratorService,
                activityUserRepository,
                lotteryRepository,
                mapper,
                activityService,
                lotteryService
            ) { }

        public async Task<Unit> Handle(
            UpdateActivityCommand request,
            CancellationToken cancellationToken
        )
        {
            var activity = await _activityRepository.FindByIdAsync(request.Id.Value);

            #region Parameter validation
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            if (request.StartTime > request.EndTime)
            {
                throw new BadRequestException("The EndTime could not be less than the StartTime.");
            }
            #endregion

            #region Database operations
            _activityRepository.Update(
                _mapper.Map<UpdateActivityCommand, Activity>(request, activity)
            );
            #endregion

            if (!await _activityRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }

            return Unit.Value;
        }
    }
}
