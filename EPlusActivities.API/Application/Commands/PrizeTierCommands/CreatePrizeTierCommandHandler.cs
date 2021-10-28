using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Utils;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class CreatePrizeTierCommandHandler : BaseCommandHandler, IRequestHandler<CreatePrizeTierCommand, PrizeTierDto>
    {
        public CreatePrizeTierCommandHandler(IFindByParentIdRepository<PrizeTier> prizeTypeRepository, IPrizeItemRepository prizeItemRepository, IActivityRepository activityRepository, IMapper mapper) : base(prizeTypeRepository, prizeItemRepository, activityRepository, mapper)
        {
        }

        public async Task<PrizeTierDto> Handle(CreatePrizeTierCommand request, CancellationToken cancellationToken)
        {
            #region  Parameter validation
            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var prizeTiers = await _prizeTierRepository.FindByParentIdAsync(
                request.ActivityId.Value
            );
            if (prizeTiers.Select(pt => pt.Percentage).Sum() + request.Percentage > 100)
            {
                throw new BadRequestException("The sum of percentages could not be greater than 100.");
            }

            if (activity.PrizeItemCount + request.PrizeItemIds.Count() > 10)
            {
                throw new BadRequestException("Could not add more than 10 prize items in an activity.");
            }
            #endregion

            #region New an entity
            activity.PrizeItemCount += request.PrizeItemIds.Count();
            var prizeTier = _mapper.Map<PrizeTier>(request);
            prizeTier.Activity = activity;

            if (request.PrizeItemIds.Count() > 0)
            {
                var prizeItems = await request.PrizeItemIds
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
    }
}
