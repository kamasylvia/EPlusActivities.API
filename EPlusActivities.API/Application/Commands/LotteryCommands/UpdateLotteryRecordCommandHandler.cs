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

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class UpdateLotteryRecordCommandHandler : BaseCommandHandler, INotificationHandler<UpdateLotteryRecordCommand>
    {
        public UpdateLotteryRecordCommandHandler(ILotteryRepository lotteryRepository, UserManager<ApplicationUser> userManager, IActivityRepository activityRepository, IPrizeItemRepository prizeItemRepository, IFindByParentIdRepository<PrizeTier> prizeTypeRepository, IMapper mapper, IFindByParentIdRepository<ActivityUser> activityUserRepository, IRepository<Coupon> couponResponseDto, ILotteryService lotteryService, IMemberService memberService, IIdGeneratorService idGeneratorService, IGeneralLotteryRecordsRepository generalLotteryRecordsRepository, IActivityService activityService) : base(lotteryRepository, userManager, activityRepository, prizeItemRepository, prizeTypeRepository, mapper, activityUserRepository, couponResponseDto, lotteryService, memberService, idGeneratorService, generalLotteryRecordsRepository, activityService)
        {
        }

        public async Task Handle(UpdateLotteryRecordCommand notification, CancellationToken cancellationToken)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(notification.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                throw new NotFoundException("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            lottery = _mapper.Map<UpdateLotteryRecordCommand, Lottery>(notification, lottery);
            lottery.PickedUpTime = notification.PickedUpTime; // Skip auto mapper.
            _lotteryRepository.Update(lottery);
            if (!await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
