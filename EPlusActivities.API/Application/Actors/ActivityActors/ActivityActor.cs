using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.ActivityCommands;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.ActivityActors
{
    public class ActivityActor : Actor, IActivityActor
    {
        private readonly ILotteryService _lotteryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityService _activityService;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IMemberService _memberService;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly IMapper _mapper;
        private readonly IActivityRepository _activityRepository;

        public ActivityActor(
            ActorHost host,
            IMemberService memberService,
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IIdGeneratorService idGeneratorService,
            IActivityUserRepository activityUserRepository,
            ILotteryRepository lotteryRepository,
            IMapper mapper,
            IActivityService activityService,
            ILotteryService lotteryService
        ) : base(host)
        {
            _lotteryService =
                lotteryService ?? throw new ArgumentNullException(nameof(lotteryService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<ActivityDto> CreateActivity(CreateActivityCommand command)
        {
            #region Database operations
            var activity = _mapper.Map<Activity>(command);
            if (
                activity.ActivityType
                is ActivityType.SingleAttendance
                    or ActivityType.SequentialAttendance
            )
            {
                activity.PrizeTiers = new List<PrizeTier>()
                {
                    new PrizeTier("Attendance") { Percentage = 100 }
                };
            }

            var activityCode = _idGeneratorService.NextId().ToString().ToCharArray();
            var replacedChar = Convert.ToChar(
                Convert.ToInt32('a' + char.GetNumericValue(activityCode.FirstOrDefault()))
            );
            activityCode[0] = replacedChar;
            activity.ActivityCode = new string(activityCode);

            activity.AvailableChannels = activity.AvailableChannels.Append(ChannelCode.Test);
            await _activityRepository.AddAsync(activity);
            if (!await _activityRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task DeleteActivity(DeleteActivityCommand command)
        {
            var activity = await _activityRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            #endregion

            #region Database operations
            var lotteries = await _lotteryRepository.FindByActivityIdAsync(command.Id.Value);
            await lotteries
                .ToAsyncEnumerable()
                .ForEachAsync(lottery => _lotteryRepository.Remove(lottery));
            _activityRepository.Remove(activity);
            if (!await _activityRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task UpdateActivity(UpdateActivityCommand command)
        {
            var activity = await _activityRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            if (command.StartTime > command.EndTime)
            {
                throw new BadRequestException("The EndTime could not be less than the StartTime.");
            }
            #endregion

            #region Database operations
            _activityRepository.Update(
                _mapper.Map<UpdateActivityCommand, Activity>(command, activity)
            );
            #endregion

            if (!await _activityRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }
        }
    }
}
