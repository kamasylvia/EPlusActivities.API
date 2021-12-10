using System;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.AttendanceCommands;
using EPlusActivities.API.Dtos.AttendanceDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Extensions;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.AttendanceActors
{
    public class AttendanceActor : Actor, IAttendanceActor
    {
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IMemberService _memberService;
        private readonly IActivityRepository _activityRepository;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceActor(
            ActorHost host,
            IAttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IIdGeneratorService idGeneratorService,
            IActivityUserRepository activityUserRepository,
            IMemberService memberService
        ) : base(host)
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
        private bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;
        public async Task<AttendanceDto> Attend(AttendCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(command.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            #endregion

            #region Update user and member
            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId.Value,
                command.UserId.Value
            );

            var today = DateTime.Now.Date;
            var attendanceDays = activityUser.AttendanceDays ?? 0;
            var sequentialAttendanceDays = activityUser.SequentialAttendanceDays ?? 0;

            if (activityUser.LastAttendanceDate == today)
            {
                throw new ConflictException("Duplicate attendance.");
            }

            #region Update LastAttendanceDate and SequentialAttendanceDays
            activityUser.SequentialAttendanceDays = IsSequential(
                activityUser.LastAttendanceDate,
                today
            )
                ? sequentialAttendanceDays + 1
                : 1;
            activityUser.LastAttendanceDate = today;
            activityUser.AttendanceDays = ++attendanceDays;
            #endregion

            #region Update credits
            var memberForUpdateCreditRequestDto = new MemberForUpdateCreditRequestDto
            {
                memberId = user.MemberId,
                points = command.EarnedCredits,
                reason = command.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Addition
            };

            var memberForUpdateCreditResponseDto = await _memberService.UpdateCreditAsync(
                command.UserId.Value,
                command.ChannelCode,
                // Enum.Parse<ChannelCode>(request.ChannelCode, true),
                memberForUpdateCreditRequestDto
            );

            activityUser.User.Credit += command.EarnedCredits;
            if (activityUser.User.Credit != memberForUpdateCreditResponseDto.Body.Content.NewPoints)
            {
                throw new RemoteServiceException(
                    "Local credits did not equal to the member's new points."
                );
            }
            #endregion

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                throw new DatabaseUpdateException(updateUserResult.ToString());
            }
            #endregion

            #region New an entity
            var attendance = _mapper.Map<Attendance>(command);
            attendance.User = user;
            attendance.Activity = activity;
            attendance.Date = DateTime.Today.ToDateOnly();
            #endregion

            #region Database operations
            _activityUserRepository.Update(activityUser);
            await _attendanceRepository.AddAsync(attendance);
            if (!await _attendanceRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}
