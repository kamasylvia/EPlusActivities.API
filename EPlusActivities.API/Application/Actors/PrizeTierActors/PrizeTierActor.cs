using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.PrizeTierCommands;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Utils;

namespace EPlusActivities.API.Application.Actors.PrizeTierActors
{
    public class PrizeTierActor : Actor, IPrizeTierActor
    {
        private readonly IMapper _mapper;
        private readonly IFindByParentIdRepository<PrizeTier> _prizeTierRepository;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IActivityRepository _activityRepository;

        public PrizeTierActor(
            ActorHost host,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) : base(host)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _prizeTierRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<PrizeTierDto> CreatePrizeTier(CreatePrizeTierCommand command)
        {
            #region  Parameter validation
            var activity = await _activityRepository.FindByIdAsync(command.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var prizeTiers = await _prizeTierRepository.FindByParentIdAsync(
                command.ActivityId.Value
            );
            if (prizeTiers.Select(pt => pt.Percentage).Sum() + command.Percentage > 100)
            {
                throw new BadRequestException(
                    "The sum of percentages could not be greater than 100."
                );
            }

            if (activity.PrizeItemCount + command.PrizeItemIds.Count() > 10)
            {
                throw new BadRequestException(
                    "Could not add more than 10 prize items in an activity."
                );
            }
            #endregion

            #region New an entity
            activity.PrizeItemCount += command.PrizeItemIds.Count();
            var prizeTier = _mapper.Map<PrizeTier>(command);
            prizeTier.Activity = activity;

            if (command.PrizeItemIds.Count() > 0)
            {
                var prizeItems = await command.PrizeItemIds
                    .ToAsyncEnumerable()
                    .SelectAwait(async id => await _prizeItemRepository.FindByIdAsync(id))
                    .Where(x => x is not null)
                    .ToListAsync();
                var prizeTierPrizeItems = new HashSet<PrizeTierPrizeItem>(
                    new HashSetReferenceEqualityComparer<PrizeTierPrizeItem>()
                );
                prizeTierPrizeItems.UnionWith(
                    prizeItems.Select(
                        pi => new PrizeTierPrizeItem { PrizeTier = prizeTier, PrizeItem = pi }
                    )
                );
                prizeTier.PrizeTierPrizeItems = prizeTierPrizeItems;
            }
            #endregion

            #region Database operations
            await _prizeTierRepository.AddAsync(prizeTier);
            if (!await _prizeTierRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<PrizeTierDto>(prizeTier);
        }

        public async Task DeletePrizeTier(DeletePrizeTierCommand command)
        {
            var tier = await _prizeTierRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (tier is null)
            {
                throw new NotFoundException("Could not find the prize type.");
            }
            #endregion

            var activity = tier.Activity;
            activity.PrizeItemCount -= tier.PrizeTierPrizeItems.Count();

            #region Database operations
            _prizeTierRepository.Remove(tier);
            if (!await _prizeTierRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task UpdatePrizeTier(UpdatePrizeTierCommand command)
        {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (prizeTier is null)
            {
                throw new NotFoundException("Could not find the prize tier.");
            }

            var prizeTypes = await _prizeTierRepository.FindByParentIdAsync(
                command.ActivityId.Value
            );
            if (
                prizeTypes.Where(pt => pt.Id != command.Id.Value).Select(pt => pt.Percentage).Sum()
                    + command.Percentage
                > 100
            )
            {
                throw new BadRequestException(
                    "The sum of percentages could not be greater than 100."
                );
            }

            var activity = prizeTier.Activity;
            var countDiff = command.PrizeItemIds.Count() - prizeTier.PrizeTierPrizeItems.Count();
            if (activity.PrizeItemCount + countDiff > 10)
            {
                throw new BadRequestException("Could not add more than 10 prizes in an activity.");
            }
            #endregion

            #region Database operations
            activity.PrizeItemCount += countDiff;
            prizeTier.PrizeTierPrizeItems = await command.PrizeItemIds
                .ToAsyncEnumerable()
                .SelectAwait(
                    async id =>
                        new PrizeTierPrizeItem
                        {
                            PrizeTierId = command.Id,
                            PrizeTier = prizeTier,
                            PrizeItemId = id,
                            PrizeItem = await _prizeItemRepository.FindByIdAsync(id)
                        }
                )
                .ToListAsync();

            _prizeTierRepository.Update(
                _mapper.Map<UpdatePrizeTierCommand, PrizeTier>(command, prizeTier)
            );
            if (!await _prizeTierRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
