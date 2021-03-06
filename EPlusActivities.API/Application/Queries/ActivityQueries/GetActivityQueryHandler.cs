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

namespace EPlusActivities.API.Application.Queries.ActivityQueries
{
    public class GetActivityQueryHandler
        : ActivityRequestHandlerBase,
          IRequestHandler<GetActivityQuery, ActivityDto>
    {
        public GetActivityQueryHandler(
            IMemberService memberService,
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IIdGeneratorService idGeneratorService,
            IActivityUserRepository activityUserRepository,
            ILotteryDetailRepository lotteryRepository,
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
            GetActivityQuery request,
            CancellationToken cancellationToken
        )
        {
            var activity = await _activityRepository.FindByIdAsync(request.Id.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            return _mapper.Map<ActivityDto>(activity);
        }
    }
}
