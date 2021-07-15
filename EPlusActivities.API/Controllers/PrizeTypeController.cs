using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
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
        public async Task<ActionResult<PrizeTypeDto>> GetAsync([FromBody] PrizeTypeDto prizeTypeDto)
        {
            #region Parameter validation
            if (prizeTypeDto.Id == Guid.Empty)
            {
                return BadRequest("The prize type ID could not be null.");
            }
            #endregion

            var prizeType = await _prizeTypeRepository.FindByIdAsync(prizeTypeDto.Id)
                            ?? await _prizeTypeRepository.FindByNameAsync(prizeTypeDto.Name);

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
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeTypeDto prizeTypeDto)
        {
            #region Parameter validation
            if (!await _prizeTypeRepository.ExistsAsync(prizeTypeDto.Id))
            {
                return BadRequest("Could not find the prize type.");
            }

            var prizeTypes = await _prizeTypeRepository.FindAllAsync();
            if (prizeTypes.Select(pt => pt.Percentage).Sum() + prizeTypeDto.Percentage >= 100)
            {
                return BadRequest("The sum of percentages could not be greater than 100.");
            }
            #endregion

            #region New an entity
            #endregion
            return Ok();
        }
    }
}
