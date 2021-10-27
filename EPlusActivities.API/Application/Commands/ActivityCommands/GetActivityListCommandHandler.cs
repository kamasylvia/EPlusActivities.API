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
    public class GetActivityListCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetActivityListCommand, IEnumerable<ActivityDto>>
    {
        public GetActivityListCommandHandler(
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

        public async Task<IEnumerable<ActivityDto>> Handle(
            GetActivityListCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (request.StartTime > request.EndTime)
            {
                throw new BadRequestException("The EndTime could not be less than the StartTime.");
            }
            #endregion

            return _mapper.Map<IEnumerable<ActivityDto>>(
                request.IsAvailable
                  ? await _activityService.GetAvailableActivitiesAsync(
                        request.AvailableChannels
                            .Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries)
                            .Select(s => Enum.Parse<ChannelCode>(s, true)),
                        request.StartTime.Value,
                        request.EndTime
                    )
                  : await _activityService.GetActivitiesAsync(
                        request.AvailableChannels
                            .Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries)
                            .Select(s => Enum.Parse<ChannelCode>(s, true)),
                        request.StartTime.Value,
                        request.EndTime
                    )
            );
        }
    }
}
