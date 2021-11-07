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
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class GetPrizeTiersByActivityIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetPrizeTiersByActivityIdCommand, IEnumerable<PrizeTierDto>>
    {
        public GetPrizeTiersByActivityIdCommandHandler(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) : base(prizeTypeRepository, prizeItemRepository, activityRepository, mapper) { }

        public async Task<IEnumerable<PrizeTierDto>> Handle(
            GetPrizeTiersByActivityIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                throw new BadRequestException("Could not find the activity.");
            }

            var prizeTiers = await _prizeTierRepository.FindByParentIdAsync(
                request.ActivityId.Value
            );

            if (prizeTiers.Count() <= 0)
            {
                throw new NotFoundException("Could not find any prize tier.");
            }

            return _mapper.Map<IEnumerable<PrizeTierDto>>(prizeTiers);
        }
    }
}
