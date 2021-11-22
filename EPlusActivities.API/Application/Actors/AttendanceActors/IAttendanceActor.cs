using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.AttendanceCommands;
using EPlusActivities.API.Dtos.AttendanceDtos;

namespace EPlusActivities.API.Application.Actors.AttendanceActors
{
    public interface IAttendanceActor : IActor
    {
        Task<AttendanceDto> Attend(AttendCommand command);
    }
}
