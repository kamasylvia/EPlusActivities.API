using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.AttendanceDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.AttendanceCommands
{
    public class AttendCommandHandler
        : BaseCommandHandler,
          IRequestHandler<AttendCommand, AttendanceDto>
    {
        public AttendCommandHandler(
            IAttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IIdGeneratorService idGeneratorService,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            IMemberService memberService
        )
            : base(
                attendanceRepository,
                userManager,
                mapper,
                activityRepository,
                idGeneratorService,
                activityUserRepository,
                memberService
            ) { }

        private bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;
        public async Task<AttendanceDto> Handle(
            AttendCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            #endregion

            #region Update user and member
            var activityUser = await _activityUserRepository.FindByIdAsync(
                request.ActivityId.Value,
                request.UserId.Value
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
                points = request.EarnedCredits,
                reason = request.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Addition
            };

            var memberForUpdateCreditResponseDto = await _memberService.UpdateCreditAsync(
                request.UserId.Value,
                request.ChannelCode,
                // Enum.Parse<ChannelCode>(request.ChannelCode, true),
                memberForUpdateCreditRequestDto
            );

            activityUser.User.Credit += request.EarnedCredits;
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
            var attendance = _mapper.Map<Attendance>(request);
            attendance.User = user;
            attendance.Activity = activity;
            attendance.Date = DateTime.Now.Date;
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
