using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.PrizeTierDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Utils;
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
        private readonly IFindByParentIdRepository<PrizeTier> _prizeTypeRepository;
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
            _prizeTypeRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTierDto>> GetByIdAsync(
            [FromBody] PrizeTierForGetByIdDto prizeTypeDto
        ) {
            var prizeTier = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);
            return prizeTier is null
                ? NotFound("Could not find the prize type.")
                : Ok(_mapper.Map<PrizeTierDto>(prizeTier));
        }

        [HttpGet("activity")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTierDto>> GetByActivityIdAsync(
            [FromBody] PrizeTierForGetByActivityIdDto prizeTypeDto
        ) {
            var activity = await _activityRepository.FindByIdAsync(prizeTypeDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var prizeTypes = await _prizeTypeRepository.FindByParentIdAsync(
                prizeTypeDto.ActivityId.Value
            );

            return prizeTypes.Count() > 0
                ? Ok(_mapper.Map<IEnumerable<PrizeTierDto>>(prizeTypes))
                : NotFound("Could not find any prize types.");
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTierDto>> CreateAsync(
            [FromBody] PrizeTierForCreateDto prizeTypeDto
        ) {
            #region  Parameter validation
            var activity = await _activityRepository.FindByIdAsync(prizeTypeDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var prizeTypes = await _prizeTypeRepository.FindByParentIdAsync(
                prizeTypeDto.ActivityId.Value
            );
            if (prizeTypes.Select(pt => pt.Percentage).Sum() + prizeTypeDto.Percentage > 100)
            {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }
            #endregion

            #region New an entity
            var prizeTier = _mapper.Map<PrizeTier>(prizeTypeDto);
            prizeTier.Activity = activity;

            if (prizeTypeDto.PrizeItemIds.Count() > 0)
            {
                var prizeItems = prizeTypeDto.PrizeItemIds.Select(
                        async id => await _prizeItemRepository.FindByIdAsync(id)
                    )
                    .Where(x => x is not null);
                var prizeTypePrizeItems = new HashSet<PrizeTierPrizeItem>(
                    new HashSetReferenceEqualityComparer<PrizeTierPrizeItem>()
                );
                prizeTypePrizeItems.UnionWith(
                    prizeItems.Select(
                        pi =>
                            new PrizeTierPrizeItem { PrizeTier = prizeTier, PrizeItem = pi.Result }
                    )
                );
                prizeTier.PrizeTierPrizeItems = prizeTypePrizeItems;
            }
            #endregion

            #region Database operations
            await _prizeTypeRepository.AddAsync(prizeTier);
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<PrizeTierDto>(prizeTier))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeTierForUpdateDto prizeTypeDto)
        {
            var prizeTier = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);

            #region Parameter validation
            if (prizeTier is null)
            {
                return BadRequest("Could not find the prize type.");
            }

            var prizeTypes = await _prizeTypeRepository.FindByParentIdAsync(
                prizeTypeDto.ActivityId.Value
            );
            if (
                prizeTypes.Where(pt => pt.Id != prizeTypeDto.Id.Value)
                    .Select(pt => pt.Percentage)
                    .Sum()
                + prizeTypeDto.Percentage
                >= 100
            ) {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }
            #endregion

            #region Database operations
            _prizeTypeRepository.Update(
                _mapper.Map<PrizeTierForUpdateDto, PrizeTier>(prizeTypeDto, prizeTier)
            );
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeTierForGetByIdDto prizeTypeDto)
        {
            var prizeTier = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);

            #region Parameter validation
            if (prizeTier is null)
            {
                return BadRequest("Could not find the prize type.");
            }
            #endregion

            #region Database operations
            _prizeTypeRepository.Remove(prizeTier);
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
