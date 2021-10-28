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
    public class DeletePrizeTierCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeletePrizeTierCommand>
    {
        public DeletePrizeTierCommandHandler(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) : base(prizeTypeRepository, prizeItemRepository, activityRepository, mapper) { }

        public async Task<Unit> Handle(
            DeletePrizeTierCommand request,
            CancellationToken cancellationToken
        )
        {
            var tier = await _prizeTierRepository.FindByIdAsync(request.Id.Value);

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

            return Unit.Value;
        }
    }
}
