using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class UpdatePrizeTierCommandHandler
        : PrizeTierRequestHandlerBase,
          IRequestHandler<UpdatePrizeTierCommand>
    {
        public UpdatePrizeTierCommandHandler(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) : base(prizeTypeRepository, prizeItemRepository, activityRepository, mapper) { }

        public async Task<Unit> Handle(
            UpdatePrizeTierCommand request,
            CancellationToken cancellationToken
        )
        {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(request.Id.Value);

            #region Parameter validation
            if (prizeTier is null)
            {
                throw new NotFoundException("Could not find the prize tier.");
            }

            var prizeTypes = await _prizeTierRepository.FindByParentIdAsync(
                request.ActivityId.Value
            );
            if (
                prizeTypes.Where(pt => pt.Id != request.Id.Value).Select(pt => pt.Percentage).Sum()
                    + request.Percentage
                > 100
            )
            {
                throw new BadRequestException(
                    "The sum of percentages could not be greater than 100."
                );
            }

            var activity = prizeTier.Activity;
            var countDiff = request.PrizeItemIds.Count() - prizeTier.PrizeTierPrizeItems.Count();
            if (activity.PrizeItemCount + countDiff > 10)
            {
                throw new BadRequestException("Could not add more than 10 prizes in an activity.");
            }
            #endregion

            #region Database operations
            activity.PrizeItemCount += countDiff;
            prizeTier.PrizeTierPrizeItems = await request.PrizeItemIds
                .ToAsyncEnumerable()
                .SelectAwait(
                    async id =>
                        new PrizeTierPrizeItem
                        {
                            PrizeTierId = request.Id,
                            PrizeTier = prizeTier,
                            PrizeItemId = id,
                            PrizeItem = await _prizeItemRepository.FindByIdAsync(id)
                        }
                )
                .ToListAsync();

            _prizeTierRepository.Update(
                _mapper.Map<UpdatePrizeTierCommand, PrizeTier>(request, prizeTier)
            );
            if (!await _prizeTierRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
