using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityDtos;
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
    public class GetActivityByCodeCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetActivityByCodeCommand, ActivityDto>
    {
        public GetActivityByCodeCommandHandler(
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
            GetActivityByCodeCommand request,
            CancellationToken cancellationToken
        )
        {
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            return _mapper.Map<ActivityDto>(activity);
        }
    }
}
