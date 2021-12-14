using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public class GetLotterySummaryStatementQueryHandler
        :
          IRequestHandler<
              GetLotterySummaryStatementQuery,
              IEnumerable<GetLotterySummaryResponse>
          >
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;
        private readonly ILotterySummaryRepository _lotterySummaryStatementRepository;

        public GetLotterySummaryStatementQueryHandler(
            IActivityRepository activityRepository,
            IMapper mapper,
            ILotterySummaryRepository lotterySummaryStatementRepository
        )
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _lotterySummaryStatementRepository = lotterySummaryStatementRepository ?? throw new ArgumentNullException(nameof(lotterySummaryStatementRepository));
        }

        public async Task<IEnumerable<GetLotterySummaryResponse>> Handle(
            GetLotterySummaryStatementQuery request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            var lotterySummaryStatement =
                await _lotterySummaryStatementRepository.FindByDateRangeAsync(
                    activity.Id.Value,
                    request.Channel,
                    request.StartDate,
                    request.EndDate
                );
            #endregion

            return _mapper.Map<IEnumerable<GetLotterySummaryResponse>>(
                lotterySummaryStatement
            );
        }
    }
}
