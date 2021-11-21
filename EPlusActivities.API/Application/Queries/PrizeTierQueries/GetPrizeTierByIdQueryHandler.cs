using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeTierQueries
{
    public class GetPrizeTierByIdQueryHandler
        : PrizeTierRequestHandlerBase,
          IRequestHandler<GetPrizeTierByIdQuery, PrizeTierDto>
    {
        public GetPrizeTierByIdQueryHandler(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) : base(prizeTypeRepository, prizeItemRepository, activityRepository, mapper) { }

        public async Task<PrizeTierDto> Handle(
            GetPrizeTierByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(request.Id.Value);
            if (prizeTier is null)
            {
                throw new NotFoundException("Could not find the prize tier.");
            }
            return _mapper.Map<PrizeTierDto>(prizeTier);
        }
    }
}
