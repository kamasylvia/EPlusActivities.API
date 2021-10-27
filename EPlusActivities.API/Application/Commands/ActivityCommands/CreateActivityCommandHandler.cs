using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        : BaseCommandHandler,
          IRequestHandler<CreateActivityCommand, ActivityDto>
    {
        public CreateActivityCommandHandler(
            IMemberService memberService,
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IIdGeneratorService idGeneratorService,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
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

        public async Task<ActivityDto> Handle(
            CreateActivityCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (request.StartTime > request.EndTime)
            {
                throw new BadRequestException("The EndTime could not be less than the StartTime.");
            }
            #endregion

            #region Database operations
            var activity = _mapper.Map<Activity>(request);
            if (
                activity.ActivityType
                is ActivityType.SingleAttendance
                    or ActivityType.SequentialAttendance
            )
            {
                activity.PrizeTiers = new List<PrizeTier>()
                {
                    new PrizeTier("Attendance") { Percentage = 100 }
                };
            }

            var activityCode = _idGeneratorService.NextId().ToString().ToCharArray();
            var replacedChar = Convert.ToChar(
                Convert.ToInt32('a' + char.GetNumericValue(activityCode.FirstOrDefault()))
            );
            activityCode[0] = replacedChar;
            activity.ActivityCode = new string(activityCode);

            activity.AvailableChannels = activity.AvailableChannels.Append(ChannelCode.Test);
            await _activityRepository.AddAsync(activity);
            if (!await _activityRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<ActivityDto>(activity);
        }
    }
}
