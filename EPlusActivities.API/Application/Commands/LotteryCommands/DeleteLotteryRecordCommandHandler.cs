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

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class DeleteLotteryRecordCommandHandler
        : LotteryRequestHandlerBase,
          IRequestHandler<DeleteLotteryRecordCommand>
    {
        public DeleteLotteryRecordCommandHandler(
            ILotteryRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IActivityUserRepository activityUserRepository,
            IRepository<Coupon> couponResponseDto,
            ILotteryService lotteryService,
            IMemberService memberService,
            IIdGeneratorService idGeneratorService,
            IGeneralLotteryRecordsRepository generalLotteryRecordsRepository,
            IActivityService activityService
        )
            : base(
                lotteryRepository,
                userManager,
                activityRepository,
                prizeItemRepository,
                prizeTypeRepository,
                mapper,
                activityUserRepository,
                couponResponseDto,
                lotteryService,
                memberService,
                idGeneratorService,
                generalLotteryRecordsRepository,
                activityService
            ) { }

        public async Task<Unit> Handle(
            DeleteLotteryRecordCommand request,
            CancellationToken cancellationToken
        )
        {
            var lottery = await _lotteryRepository.FindByIdAsync(request.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                throw new NotFoundException("Could not find the the lottery.");
            }
            #endregion

            #region Database operations
            _lotteryRepository.Remove(lottery);
            if (await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
