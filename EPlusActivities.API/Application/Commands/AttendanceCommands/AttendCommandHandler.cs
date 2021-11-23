using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.AttendanceActors;
using EPlusActivities.API.Dtos.AttendanceDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AttendanceCommands
{
    public class AttendCommandHandler : IRequestHandler<AttendCommand, AttendanceDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public AttendCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        private bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;
        public async Task<AttendanceDto> Handle(
            AttendCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IAttendanceActor>(
                    new ActorId(command.UserId.ToString()),
                    nameof(AttendanceActor)
                )
                .Attend(command);
    }
}
