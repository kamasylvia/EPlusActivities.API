using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.PrizeTypeDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class PrizeTypeController : Controller
    {
        private readonly IFindByUserIdRepository<PrizeType> _prizeTypeRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;

        public PrizeTypeController(
            IFindByUserIdRepository<PrizeType> prizeTypeRepository,
            IActivityRepository activityRepository,
            IMapper mapper
        )
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _prizeTypeRepository = prizeTypeRepository
                ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
            _activityRepository = activityRepository
                ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTypeDto>> GetByIdAsync([FromBody] PrizeTypeForGetByIdDto prizeTypeDto)
        {
            var prizeType = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);
            return prizeType is null
                ? NotFound("Could not find the prize type.")
                : Ok(_mapper.Map<PrizeTypeDto>(prizeType));
        }

        [HttpGet("activity")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTypeDto>> GetByActivityIdAsync([FromBody] PrizeTypeForGetByActivityIdDto prizeTypeDto)
        {
            var activity = await _activityRepository.FindByIdAsync(prizeTypeDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var prizeTypes = await _prizeTypeRepository.FindByUserIdAsync(prizeTypeDto.ActivityId.Value);

            return prizeTypes.Count() > 0
                ? Ok(_mapper.Map<IEnumerable<PrizeTypeDto>>(prizeTypes))
                : NotFound("Could not find any prize types.");
        }


        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTypeDto>> CreateAsync([FromBody] PrizeTypeForCreateDto prizeTypeDto)
        {
            #region  Parameter validation
            var activity = await _activityRepository.FindByIdAsync(prizeTypeDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var prizeTypes = await _prizeTypeRepository.FindByUserIdAsync(prizeTypeDto.ActivityId.Value);
            if (prizeTypes.Select(pt => pt.Percentage).Sum() + prizeTypeDto.Percentage >= 100)
            {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }
            #endregion

            #region New an entity
            var prizeType = _mapper.Map<PrizeType>(prizeTypeDto);
            prizeType.Activity = activity;
            #endregion

            #region Database operations
            await _prizeTypeRepository.AddAsync(prizeType);
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<PrizeTypeDto>(prizeType))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeTypeForUpdateDto prizeTypeDto)
        {
            var prizeType = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);

            #region Parameter validation
            if (prizeType is null)
            {
                return BadRequest("Could not find the prize type.");
            }

            var prizeTypes = await _prizeTypeRepository.FindByUserIdAsync(prizeTypeDto.ActivityId.Value);
            if (prizeTypes.Where(pt => pt.Id != prizeTypeDto.Id.Value)
                          .Select(pt => pt.Percentage)
                          .Sum() + prizeTypeDto.Percentage >= 100)
            {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }
            #endregion

            #region Database operations
            _prizeTypeRepository.Update(_mapper.Map<PrizeTypeForUpdateDto, PrizeType>(
                prizeTypeDto,
                prizeType));
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeTypeForGetByIdDto prizeTypeDto)
        {
            var prizeType = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);

            #region Parameter validation
            if (prizeType is null)
            {
                return BadRequest("Could not find the prize type.");
            }
            #endregion

            #region Database operations
            _prizeTypeRepository.Remove(prizeType);
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
