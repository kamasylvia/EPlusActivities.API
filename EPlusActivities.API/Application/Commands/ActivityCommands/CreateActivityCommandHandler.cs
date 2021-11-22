using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityActors;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Infrastructure.Exceptions;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityCommands
{
    public class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, ActivityDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateActivityCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<ActivityDto> Handle(
            CreateActivityCommand command,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (command.StartTime > command.EndTime)
            {
                throw new BadRequestException("The EndTime could not be less than the StartTime.");
            }
            #endregion

            return await _actorProxyFactory
                .CreateActorProxy<IActivityActor>(
                    new ActorId(command.Name + command.StartTime.ToString()),
                    nameof(ActivityActor)
                )
                .CreateActivity(command);
        }
    }
}
