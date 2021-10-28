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
    public class GetPrizeTierByIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetPrizeTierByIdCommand, PrizeTierDto>
    {
        public GetPrizeTierByIdCommandHandler(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) : base(prizeTypeRepository, prizeItemRepository, activityRepository, mapper) { }

        public async Task<PrizeTierDto> Handle(
            GetPrizeTierByIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(request.Id.Value);
            if (prizeTier is null)
            {
                throw new NotFoundException("Could not find the prize type.");
            }
            return _mapper.Map<PrizeTierDto>(prizeTier);
        }
    }
}
