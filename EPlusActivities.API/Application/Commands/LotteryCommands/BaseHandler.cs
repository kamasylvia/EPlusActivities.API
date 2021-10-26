using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public abstract class BaseHandler
    {
        protected readonly IMapper _mapper;
        protected readonly ILotteryRepository _lotteryRepository;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IActivityRepository _activityRepository;
        protected readonly IPrizeItemRepository _prizeItemRepository;
        protected readonly IFindByParentIdRepository<ActivityUser> _activityUserRepository;
        protected readonly IRepository<Coupon> _couponRepository;
        protected readonly IFindByParentIdRepository<PrizeTier> _prizeTypeRepository;
        protected readonly ILotteryService _lotteryService;
        protected readonly IIdGeneratorService _idGeneratorService;
        protected readonly IGeneralLotteryRecordsRepository _generalLotteryRecordsRepository;
        protected readonly IMemberService _memberService;
        protected readonly IActivityService _activityService;

        protected BaseHandler(
            ILotteryRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            IRepository<Coupon> couponResponseDto,
            ILotteryService lotteryService,
            IMemberService memberService,
            IIdGeneratorService idGeneratorService,
            IGeneralLotteryRecordsRepository generalLotteryRecordsRepository,
            IActivityService activityService
        )
        {
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _generalLotteryRecordsRepository =
                generalLotteryRecordsRepository
                ?? throw new ArgumentNullException(nameof(generalLotteryRecordsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _lotteryService =
                lotteryService ?? throw new ArgumentNullException(nameof(lotteryService));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _couponRepository =
                couponResponseDto ?? throw new ArgumentNullException(nameof(couponResponseDto));
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _prizeTypeRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
        }
    }
}
