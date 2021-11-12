using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.AttendanceDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.AttendanceCommands
{
    public class GetAttendanceRecordsCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetAttendanceRecordsCommand, IEnumerable<AttendanceDto>>
    {
        public GetAttendanceRecordsCommandHandler(
            IAttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IIdGeneratorService idGeneratorService,
            IActivityUserRepository activityUserRepository,
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

        public async Task<IEnumerable<AttendanceDto>> Handle(
            GetAttendanceRecordsCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            if (!await _activityRepository.ExistsAsync(request.ActivityId.Value))
            {
                throw new NotFoundException("Could not find the activity.");
            }
            #endregion

            var attendanceRecord = await _attendanceRepository.FindByUserIdAsync(
                userId: request.UserId.Value,
                activityId: request.ActivityId.Value,
                startDate: request.StartDate.Value,
                endDate: request.EndDate
            );

            if (attendanceRecord.Count() <= 0)
            {
                throw new NotFoundException("Could not find any attendances.");
            }

            return _mapper.Map<IEnumerable<AttendanceDto>>(attendanceRecord);
        }
    }
}
