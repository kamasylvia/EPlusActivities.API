using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.LotteryDtos;
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
    public class GetWinningRecordsByUserIdCommandHandler : BaseCommandHandler, IRequestHandler<GetWinningRecordsByUserIdCommand, IEnumerable<LotteryDto>>
    {
        public GetWinningRecordsByUserIdCommandHandler(ILotteryRepository lotteryRepository, UserManager<ApplicationUser> userManager, IActivityRepository activityRepository, IPrizeItemRepository prizeItemRepository, IFindByParentIdRepository<PrizeTier> prizeTypeRepository, IMapper mapper, IFindByParentIdRepository<ActivityUser> activityUserRepository, IRepository<Coupon> couponResponseDto, ILotteryService lotteryService, IMemberService memberService, IIdGeneratorService idGeneratorService, IGeneralLotteryRecordsRepository generalLotteryRecordsRepository, IActivityService activityService) : base(lotteryRepository, userManager, activityRepository, prizeItemRepository, prizeTypeRepository, mapper, activityUserRepository, couponResponseDto, lotteryService, memberService, idGeneratorService, generalLotteryRecordsRepository, activityService)
        {
        }

        public async Task<IEnumerable<LotteryDto>> Handle(GetWinningRecordsByUserIdCommand request, CancellationToken cancellationToken)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            var records = await FindLotteryRecordsAsync(request.UserId.Value);
            return records.Where(record => record.IsLucky);
        }
    }
}
