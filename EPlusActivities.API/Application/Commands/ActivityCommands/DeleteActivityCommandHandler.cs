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
    public class DeleteActivityCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeleteActivityCommand>
    {
        public DeleteActivityCommandHandler(
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

        public async Task<Unit> Handle(
            DeleteActivityCommand request,
            CancellationToken cancellationToken
        )
        {
            var activity = await _activityRepository.FindByIdAsync(request.Id.Value);

            #region Parameter validation
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            #endregion

            #region Database operations
            var lotteries = await _lotteryRepository.FindByActivityIdAsync(request.Id.Value);
            await lotteries
                .ToAsyncEnumerable()
                .ForEachAsync(lottery => _lotteryRepository.Remove(lottery));
            _activityRepository.Remove(activity);
            if (!await _activityRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
