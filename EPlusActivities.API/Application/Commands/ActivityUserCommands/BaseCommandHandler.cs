using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public abstract class BaseCommandHandler
    {
        protected readonly IActivityRepository _activityRepository;
        protected readonly IMemberService _memberService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;
        protected readonly IActivityUserRepository _activityUserRepository;
        protected readonly IIdGeneratorService _idGeneratorService;
        protected readonly IGeneralLotteryRecordsRepository _statementRepository;
        protected readonly IActivityService _activityService;

        protected BaseCommandHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
        )
        {
            _statementRepository =
                statementRepository ?? throw new ArgumentNullException(nameof(statementRepository));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
        }
    }
}
