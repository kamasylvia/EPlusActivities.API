using System;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application
{
    public abstract class AttendanceRequestHandlerBase
    {
        protected readonly IMapper _mapper;
        protected readonly IAttendanceRepository _attendanceRepository;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IActivityUserRepository _activityUserRepository;
        protected readonly IActivityRepository _activityRepository;
        protected readonly IIdGeneratorService _idGeneratorService;
        protected readonly IMemberService _memberService;
        public AttendanceRequestHandlerBase(
            IAttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IIdGeneratorService idGeneratorService,
            IActivityUserRepository activityUserRepository,
            IMemberService memberService
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _attendanceRepository =
                attendanceRepository
                ?? throw new ArgumentNullException(nameof(attendanceRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
    }
}
