using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public abstract class BaseCommandHandler
    {
        protected readonly IFindByParentIdRepository<PrizeTier> _prizeTierRepository;
        protected readonly IPrizeItemRepository _prizeItemRepository;
        protected readonly IActivityRepository _activityRepository;
        protected readonly IMapper _mapper;

        protected BaseCommandHandler(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _prizeTierRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }
    }
}
