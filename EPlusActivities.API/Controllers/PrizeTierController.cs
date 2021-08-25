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
            #endregion

            #region New an entity
            var prizeTier = _mapper.Map<PrizeTier>(prizeTierDto);
            prizeTier.Activity = activity;

            if (prizeTierDto.PrizeItemIds.Count() > 0)
            {
                var prizeItems = prizeTierDto.PrizeItemIds.Select(
                        async id => await _prizeItemRepository.FindByIdAsync(id)
                    )
                    .Where(x => x is not null);
                var prizeTierPrizeItems = new HashSet<PrizeTierPrizeItem>(
                    new HashSetReferenceEqualityComparer<PrizeTierPrizeItem>()
                );
                prizeTierPrizeItems.UnionWith(
                    prizeItems.Select(
                        pi =>
                            new PrizeTierPrizeItem { PrizeTier = prizeTier, PrizeItem = pi.Result }
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
            #endregion

            #region Database operations
            _prizeTierRepository.Update(
                _mapper.Map<PrizeTierForUpdateDto, PrizeTier>(prizeTierDto, prizeTier)
            );
            var succeeded = await _prizeTierRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeTierForGetByIdDto prizeTierDto)
        {
            var prizeTier = await _prizeTierRepository.FindByIdAsync(prizeTierDto.Id.Value);

            #region Parameter validation
            if (prizeTier is null)
            {
                return BadRequest("Could not find the prize type.");
            }
            #endregion

            #region Database operations
            _prizeTierRepository.Remove(prizeTier);
            var succeeded = await _prizeTierRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
