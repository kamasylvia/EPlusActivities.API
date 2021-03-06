using System;
using AutoMapper;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.DrawingActors
{
    public partial class DrawingActor : Actor, IDrawingActor
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly ILotteryDetailRepository _lotteryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityRepository _activityRepository;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IFindByParentIdRepository<PrizeTier> _prizeTypeRepository;
        private readonly IMapper _mapper;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly IRepository<Coupon> _couponRepository;
        private readonly ILotteryService _lotteryService;
        private readonly IMemberService _memberService;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly ILotterySummaryRepository _lotterySummaryStatementRepository;
        private readonly IActivityService _activityService;

        public DrawingActor(
            ActorHost host,
            IActorProxyFactory actorProxyFactory,
            ILotteryDetailRepository lotteryRepository,
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
            ILotterySummaryRepository lotterySummaryStatementRepository,
            IActivityService activityService
        ) : base(host)
        {
            _actorProxyFactory = actorProxyFactory;
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _prizeTypeRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _couponRepository =
                couponResponseDto ?? throw new ArgumentNullException(nameof(couponResponseDto));
            _lotteryService =
                lotteryService ?? throw new ArgumentNullException(nameof(lotteryService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _lotterySummaryStatementRepository =
                lotterySummaryStatementRepository
                ?? throw new ArgumentNullException(nameof(lotterySummaryStatementRepository));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
        }
    }
}
