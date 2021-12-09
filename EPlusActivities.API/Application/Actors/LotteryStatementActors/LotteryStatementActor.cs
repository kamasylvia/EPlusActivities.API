using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.LotteryStatementCommands;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Application.Actors.LotteryStatementActors
{
    public class LotteryStatementActor : Actor, ILotteryStatementActor, IRemindable
    {
        private readonly ILogger<LotteryStatementActor> _logger;
        private readonly IMapper _mapper;
        private readonly IGeneralLotteryRecordsRepository _generalLotteryRecordsRepository;
        private readonly IActivityRepository _activityRepository;

        public LotteryStatementActor(
            ActorHost host,
            ILogger<LotteryStatementActor> logger,
            IMapper mapper,
            IGeneralLotteryRecordsRepository generalLotteryRecordsRepository,
            IActivityRepository activityRepository
        ) : base(host)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _generalLotteryRecordsRepository =
                generalLotteryRecordsRepository
                ?? throw new ArgumentNullException(nameof(generalLotteryRecordsRepository));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task CreateGeneralLotteryStatementAsync(
            CreateGeneralLotteryStatementCommand command
        )
        {
            var activity = await _activityRepository.FindByIdAsync(command.ActivityId);
            var statement = _mapper.Map<GeneralLotteryRecords>(command);
            statement.Activity = activity;
            await _generalLotteryRecordsRepository.AddAsync(statement);

            if (!await _generalLotteryRecordsRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Failed to create a new GeneralLotteryRecords");
            }
        }

        public async Task SetReminderAsync() =>
            await this.RegisterReminderAsync(
                "StatementGeneratorReminder",
                null,
                DateTime.Today.AddHours(23).AddMinutes(50) - DateTime.Now,
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
                    await CreateGeneralLotteryStatementAsync(
                        new CreateGeneralLotteryStatementCommand
                        {
                            ActivityId = activity.Id.Value,
                            Channel = channel,
                            DateTime = DateTime.Today
                        }
                    );
                }
            }
        }

        public async Task UpdateGeneralLotteryStatementAsync(
            UpdateGeneralLotteryStatementCommand command
        )
        {
            var statement = await _generalLotteryRecordsRepository.FindByDateAsync(
                command.ActivityId,
                command.Channel,
                command.DateTime
            );

            statement.Draws += command.Draws;
            statement.Redemption += command.Redemption;
            statement.Winners += command.Winners;

            _generalLotteryRecordsRepository.Update(statement);
            if (!await _generalLotteryRecordsRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Failed to update the GeneralLotteryRecords");
            }
        }
    }
}
