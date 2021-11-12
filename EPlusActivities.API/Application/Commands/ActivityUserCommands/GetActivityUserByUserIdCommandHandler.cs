using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class GetActivityUserByUserIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetActivityUserByUserIdCommand, IEnumerable<ActivityUserDto>>
    {
        public GetActivityUserByUserIdCommandHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
        )
            : base(
                activityRepository,
                memberService,
                userManager,
                activityUserRepository,
                mapper,
                idGeneratorService,
                activityService,
                statementRepository
            ) { }

        public async Task<IEnumerable<ActivityUserDto>> Handle(
            GetActivityUserByUserIdCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            return _mapper.Map<IEnumerable<ActivityUserDto>>(
                await _activityService.GetAvailableActivityUserLinksAsync(
                    request.UserId.Value,
                    request.AvailableChannel,
                    request.StartTime,
                    request.EndTime
                )
            );
        }
    }
}
