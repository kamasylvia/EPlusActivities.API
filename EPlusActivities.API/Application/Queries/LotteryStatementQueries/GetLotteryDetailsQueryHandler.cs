using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.LotteryStatementService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public class GetLotteryDetailsQueryHandler
        : IRequestHandler<GetLotteryDetailsQuery, IEnumerable<GetLotteryDetailsResponse>>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ILotteryDetailRepository _lotteryDetailRepository;
        private readonly ILotteryStatementService _lotteryStatementService;

        public GetLotteryDetailsQueryHandler(
            IActivityRepository activityRepository,
            ILotteryDetailRepository lotteryDetailRepository,
            ILotteryStatementService lotteryStatementService
        )
        {
            _activityRepository = activityRepository;
            _lotteryDetailRepository = lotteryDetailRepository;
            _lotteryStatementService = lotteryStatementService;
        }

        public async Task<IEnumerable<GetLotteryDetailsResponse>> Handle(
            GetLotteryDetailsQuery request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var lotteryDetails = await _lotteryDetailRepository.FindByDateRangeAsync(
                activity.Id.Value,
                request.Channel,
                request.StartTime,
                request.EndTime
            );
            #endregion

            return _lotteryStatementService.CreateLotteryDetailStatement(lotteryDetails);
        }
    }
}
