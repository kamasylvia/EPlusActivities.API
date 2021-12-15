using System;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.LotteryStatementCommands;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Extensions;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Application.Actors.LotteryStatementActors
{
    public class LotteryStatementActor : Actor, ILotteryStatementActor, IRemindable
    {
        private readonly ILogger<LotteryStatementActor> _logger;
        private readonly IMapper _mapper;
        private readonly ILotterySummaryRepository _lotterySummaryStatementRepository;
        private readonly IActivityRepository _activityRepository;

        public LotteryStatementActor(
            ActorHost host,
            ILogger<LotteryStatementActor> logger,
            IMapper mapper,
            ILotterySummaryRepository lotterySummaryStatementRepository,
            IActivityRepository activityRepository
        ) : base(host)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _lotterySummaryStatementRepository =
                lotterySummaryStatementRepository
                ?? throw new ArgumentNullException(nameof(lotterySummaryStatementRepository));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task CreateLotterySummaryStatementAsync(
            CreateLotterySummaryStatementCommand command
        )
        {
            var activity = await _activityRepository.FindByIdAsync(command.ActivityId);
            var statement = _mapper.Map<LotterySummary>(command);
            statement.Activity = activity;
            await _lotterySummaryStatementRepository.AddAsync(statement);

            if (!await _lotterySummaryStatementRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Failed to create a new LotterySummaryStatement");
            }
        }

        public async Task SetReminderAsync() =>
            await this.RegisterReminderAsync(
                "StatementGeneratorReminder",
                null,
                new TimeOnly(23, 50) - DateTime.Now.ToTimeOnly(),
                TimeSpan.FromDays(1)
            );

        public async Task ReceiveReminderAsync(
            string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period
        )
        {
            var availableActivitiesOnTomorrow =
                await _activityRepository.FindAvailableActivitiesAsync(DateTime.Today.AddDays(1));
            foreach (var activity in availableActivitiesOnTomorrow)
            {
                foreach (var channel in activity.AvailableChannels)
                {
                    await CreateLotterySummaryStatementAsync(
                        new CreateLotterySummaryStatementCommand
                        {
                            ActivityId = activity.Id.Value,
                            Channel = channel,
                            Date = DateTime.Today
                        }
                    );
                }
            }
        }

        public async Task UpdateLotterySummaryStatementAsync(
            UpdateLotterySummaryStatementCommand command
        )
        {
            var statement = await _lotterySummaryStatementRepository.FindByDateAsync(
                command.ActivityId,
                command.Channel,
                command.Date.ToDateOnly()
            );

            statement.Draws += command.Draws;
            statement.Redemption += command.Redemption;
            statement.Winners += command.Winners;

            _lotterySummaryStatementRepository.Update(statement);
            if (!await _lotterySummaryStatementRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Failed to update the LotterySummaryStatement");
            }
        }
    }
}
