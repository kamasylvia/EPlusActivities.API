using System;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application
{
    public abstract class ActivityRequestHandlerBase
    {
        protected readonly IActivityRepository _activityRepository;
        protected readonly IMemberService _memberService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IIdGeneratorService _idGeneratorService;
        protected readonly IActivityUserRepository _activityUserRepository;
        protected readonly ILotteryRepository _lotteryRepository;
        protected readonly IMapper _mapper;
        protected readonly IActivityService _activityService;
        protected readonly ILotteryService _lotteryService;
        protected ActivityRequestHandlerBase(
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
        {
            _lotteryService =
                lotteryService ?? throw new ArgumentNullException(nameof(lotteryService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }
    }
}
