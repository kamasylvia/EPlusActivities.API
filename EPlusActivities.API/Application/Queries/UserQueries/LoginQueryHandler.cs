using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.UserActors;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Queries.UserQueries
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public LoginQueryHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<UserDto> Handle(LoginQuery request, CancellationToken cancellationToken) => await _actorProxyFactory
                   .CreateActorProxy<IUserActor>(new ActorId(request.UserId.ToString() + request.ChannelCode.ToString()), nameof(UserActor))
                   .Login(request);
    }
}
