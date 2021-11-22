using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Queries.ActivityUserQueries;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.ActivityService;
using Microsoft.AspNetCore.Identity;
using EPlusActivities.API.Application.Queries.UserQueries;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.MemberService;
using EPlusActivities.API.Services.IdGeneratorService;

namespace EPlusActivities.API.Application.Actors.ActivityUserActors
{
    public class ActivityUserActor : Actor, IActivityUserActor
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityService _activityService;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGeneratorService;

        public ActivityUserActor(
            ActorHost host,
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
        ) : base(host)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository = activityUserRepository;
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _idGeneratorService = idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
        }

       public async Task BindActivityAndUser(BindActivityAndUserCommand command)
       {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.Value.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(command.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId.Value,
                command.UserId.Value
            );
            if (activityUser is not null)
            {
                throw new ConflictException("The user had already joined the activity.");
            }
            #endregion

            #region Create an ActivityUser link
            activityUser = new ActivityUser { Activity = activity, User = user, };
            #endregion

            #region Database operations
            await _activityUserRepository.AddAsync(activityUser);
            if (!await _activityUserRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
       }
        public async Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdQuery request
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var bound = (await StateManager.TryGetStateAsync<bool>("bound")).Value;
            #endregion

            return bound
              ? _mapper.Map<IEnumerable<ActivityUserDto>>(
                    await _activityService.GetAvailableActivityUserLinksAsync(
                        request.UserId.Value,
                        request.AvailableChannel,
                        request.StartTime,
                        request.EndTime
                    )
                )
              : new List<ActivityUserDto>();
        }

        public async Task<bool> BindUserWithAvailableActivitiesAsync(LoginQuery request)
        {
            var bound = (await StateManager.TryGetStateAsync<bool>("bound")).Value;
            if (!bound)
            {
                bound = await StateManager.AddOrUpdateStateAsync(
                    "bound",
                    true,
                    (key, currentState) => true
                );

                var newCreatedLinks = await _activityService.BindUserWithAvailableActivities(
                    request.UserId.Value,
                    request.ChannelCode
                );
            }
            return bound;
        }
    }
}
