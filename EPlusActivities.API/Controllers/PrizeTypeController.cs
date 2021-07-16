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
        private readonly INameExistsRepository<PrizeType> _prizeTypeRepository;

        private readonly IMapper _mapper;

        public PrizeTypeController(
            INameExistsRepository<PrizeType> prizeTypeRepository,
            IMapper mapper
        )
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _prizeTypeRepository = prizeTypeRepository
                ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTypeDto>> GetByIdAsync([FromBody] PrizeTypeForGetByIdDto prizeTypeDto)
        {
            var prizeType = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);
            // ?? await _prizeTypeRepository.FindByNameAsync(prizeTypeDto.Name);

            return prizeType is null
                ? NotFound("Could not find the prize type.")
                : Ok(_mapper.Map<PrizeTypeDto>(prizeType));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTypeDto>> GetByNameAsync([FromBody] PrizeTypeForGetByNameDto prizeTypeDto)
        {
            var prizeType = await _prizeTypeRepository.FindByNameAsync(prizeTypeDto.Name);

            return prizeType is null
                ? NotFound("Could not find the prize type.")
                : Ok(_mapper.Map<PrizeTypeDto>(prizeType));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeTypeDto>> CreateAsync([FromBody] PrizeTypeDto prizeTypeDto)
        {
            #region  Parameter validation
            if (await _prizeTypeRepository.ExistsAsync(prizeTypeDto.Id)
                || await _prizeTypeRepository.ExistsAsync(prizeTypeDto.Name))
            {
                return BadRequest("This prize type is already existed.");
            }
            #endregion

            #region New an entity
            var prizeType = _mapper.Map<PrizeType>(prizeTypeDto);
            prizeType.Id = Guid.NewGuid();
            #endregion

            #region Database operations
            await _prizeTypeRepository.AddAsync(prizeType);
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeTypeForUpdateDto prizeTypeDto)
        {
            #region Parameter validation
            if (!await _prizeTypeRepository.ExistsAsync(prizeTypeDto.Id.Value))
            {
                return BadRequest("Could not find the prize type.");
            }

            var prizeTypes = await _prizeTypeRepository.FindAllAsync();
            if (prizeTypes.Select(pt => pt.Percentage).Sum() + prizeTypeDto.Percentage >= 100)
            {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }
            #endregion

            #region Database operations
            var prizeType = _mapper.Map<PrizeTypeForUpdateDto, PrizeType>(
                prizeTypeDto,
                await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value));
            _prizeTypeRepository.Update(prizeType);
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
            #region Parameter validation
            if (await _prizeTypeRepository.ExistsAsync(prizeTypeDto.Id.Value))
            {
                return BadRequest("Could not find the prize type.");
            }
            #endregion

            #region Database operations
            var prizeType = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id.Value);
            _prizeTypeRepository.Remove(prizeType);
            var succeeded = await _prizeTypeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
