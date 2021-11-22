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

namespace EPlusActivities.API.Application.Queries.AttendanceQueries
{
    public class GetAttendanceQueryHandler
        : AttendanceRequestHandlerBase,
          IRequestHandler<GetAttendanceQuery, AttendanceDto>
    {
        public GetAttendanceQueryHandler(
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

        public async Task<AttendanceDto> Handle(
            GetAttendanceQuery request,
            CancellationToken cancellationToken
        )
        {
            var attendance = await _attendanceRepository.FindByIdAsync(request.Id.Value);
            if (attendance is null)
            {
                throw new NotFoundException("Could not find the attendance.");
            }
            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}
