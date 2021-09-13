using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 奖品档次 API
    /// </summary>
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class PrizeTierController : Controller
    {
        private readonly IFindByParentIdRepository<PrizeTier> _prizeTierRepository;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;

        public PrizeTierController(
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IPrizeItemRepository prizeItemRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _prizeTierRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        /// <summary>
        /// 获取奖品档次
        /// </summary>
        /// <param name="prizeTierDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeTierDto>> GetByIdAsync(
            [FromQuery] PrizeTierForGetByIdDto prizeTierDto
        ) {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(prizeTierDto.Id.Value);
            return prizeTier is null
                ? NotFound("Could not find the prize type.")
                : Ok(_mapper.Map<PrizeTierDto>(prizeTier));
        }

        /// <summary>
        /// 获取某个活动的所有奖品档次
        /// </summary>
        /// <param name="prizeTierDto"></param>
        /// <returns></returns>
        [HttpGet("activity")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeTierDto>> GetByActivityIdAsync(
            [FromQuery] PrizeTierForGetByActivityIdDto prizeTierDto
        ) {
            var activity = await _activityRepository.FindByIdAsync(prizeTierDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var prizeTiers = await _prizeTierRepository.FindByParentIdAsync(
                prizeTierDto.ActivityId.Value
            );

            return prizeTiers.Count() > 0
                ? Ok(_mapper.Map<IEnumerable<PrizeTierDto>>(prizeTiers))
                : NotFound("Could not find any prize types.");
        }

        /// <summary>
        /// 新建奖品档次
        /// </summary>
        /// <param name="prizeTierDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeTierDto>> CreateAsync(
            [FromBody] PrizeTierForCreateDto prizeTierDto
        ) {
            #region  Parameter validation
            var activity = await _activityRepository.FindByIdAsync(prizeTierDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var prizeTiers = await _prizeTierRepository.FindByParentIdAsync(
                prizeTierDto.ActivityId.Value
            );
            if (prizeTiers.Select(pt => pt.Percentage).Sum() + prizeTierDto.Percentage > 100)
            {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }

            if (activity.PrizeItemCount + prizeTierDto.PrizeItemIds.Count() > 10)
            {
                return BadRequest("Could not add more than 10 prize items in an activity.");
            }
            #endregion

            #region New an entity
            activity.PrizeItemCount += prizeTierDto.PrizeItemIds.Count();
            var prizeTier = _mapper.Map<PrizeTier>(prizeTierDto);
            prizeTier.Activity = activity;

            if (prizeTierDto.PrizeItemIds.Count() > 0)
            {
                var prizeItems = await prizeTierDto.PrizeItemIds.ToAsyncEnumerable()
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
            var succeeded = await _prizeTierRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<PrizeTierDto>(prizeTier))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 修改奖品档次
        /// </summary>
        /// <param name="prizeTierDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeTierForUpdateDto prizeTierDto)
        {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(prizeTierDto.Id.Value);

            #region Parameter validation
            if (prizeTier is null)
            {
                return BadRequest("Could not find the prize type.");
            }

            var prizeTypes = await _prizeTierRepository.FindByParentIdAsync(
                prizeTierDto.ActivityId.Value
            );
            if (
                prizeTypes.Where(pt => pt.Id != prizeTierDto.Id.Value)
                    .Select(pt => pt.Percentage)
                    .Sum()
                + prizeTierDto.Percentage
                >= 100
            ) {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }

            var activity = prizeTier.Activity;
            var countDiff =
                prizeTierDto.PrizeItemIds.Count() - prizeTier.PrizeTierPrizeItems.Count();
            if (activity.PrizeItemCount + countDiff > 10)
            {
                return BadRequest("Could not add more than 10 prizes in an activity.");
            }
            #endregion

            #region Database operations
            activity.PrizeItemCount += countDiff;
            prizeTier.PrizeTierPrizeItems = await prizeTierDto.PrizeItemIds.ToAsyncEnumerable()
                .SelectAwait(
                    async id =>
                        new PrizeTierPrizeItem
                        {
                            PrizeTierId = prizeTierDto.Id,
                            PrizeTier = prizeTier,
                            PrizeItemId = id,
                            PrizeItem = await _prizeItemRepository.FindByIdAsync(id)
                        }
                )
                .ToListAsync();

            _prizeTierRepository.Update(
                _mapper.Map<PrizeTierForUpdateDto, PrizeTier>(prizeTierDto, prizeTier)
            );
            var succeeded = await _prizeTierRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 删除奖品档次，删除后其所属的活动会自动更新奖品数量。
        /// </summary>
        /// <param name="prizeTierDto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeTierForGetByIdDto prizeTierDto)
        {
            var tier = await _prizeTierRepository.FindByIdAsync(prizeTierDto.Id.Value);

            #region Parameter validation
            if (tier is null)
            {
                return BadRequest("Could not find the prize type.");
            }
            #endregion

            var activity = tier.Activity;
            activity.PrizeItemCount -= tier.PrizeTierPrizeItems.Count();

            #region Database operations
            _prizeTierRepository.Remove(tier);
            var succeeded = await _prizeTierRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
