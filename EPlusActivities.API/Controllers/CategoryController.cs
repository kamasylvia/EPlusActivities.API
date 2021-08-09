using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.CategoryDtos;
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
    public class CategoryController : Controller
    {
        private readonly INameExistsRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) {
            _categoryRepository =
                categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<CategoryDto>> GetByIdAsync(
            [FromBody] CategoryForGetByIdDto categoryDto
        ) {
            var category = await _categoryRepository.FindByIdAsync(categoryDto.Id.Value);
            return category is null ? NotFound($"Could not find the category.") : Ok(category);
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<CategoryDto>> GetByNameAsync(
            [FromBody] CategoryForGetByNameDto categoryDto
        ) {
            var category = await _categoryRepository.FindByNameAsync(categoryDto.Name);
            return category is null
                ? NotFound($"Could not find the category.")
                : Ok(_mapper.Map<CategoryDto>(category));
        }

        [HttpGet("search")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByContainedAsync(
            [FromBody] CategoryForGetByNameDto categoryDto
        ) {
            var categories = await _categoryRepository.FindByContainedNameAsync(categoryDto.Name);
            return categories.Count() > 0
                ? Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories))
                : NotFound($"Could not find any category.");
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<CategoryDto>> CreateAsync(
            [FromBody] CategoryForGetByNameDto categoryDto
        ) {
            #region Parameter validation
            if (await _categoryRepository.ExistsAsync(categoryDto.Name))
            {
                return Conflict($"The category '{categoryDto.Name}' is already existed.");
            }
            #endregion

            #region New an entity
            var category = _mapper.Map<Category>(categoryDto);
            #endregion

            #region Database operations
            await _categoryRepository.AddAsync(category);
            var succeeded = await _categoryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<CategoryDto>(category))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPatch("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateNameAsync([FromBody] CategoryDto categoryDto)
        {
            var category = await _categoryRepository.FindByIdAsync(categoryDto.Id.Value);

            #region Parameter validation
            if (category is null)
            {
                return NotFound($"Could not find the category with ID '{categoryDto.Id}'");
            }

            if (await _categoryRepository.ExistsAsync(categoryDto.Name))
            {
                return Conflict($"The category '{categoryDto.Name}' is already existed.");
            }
            #endregion

            #region Database operations
            _categoryRepository.Update(_mapper.Map<CategoryDto, Category>(categoryDto, category));
            var succeeded = await _categoryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] CategoryDto categoryDto)
        {
            #region Parameter validation
            if (
                !await _categoryRepository.ExistsAsync(categoryDto.Id.Value)
                || !await _categoryRepository.ExistsAsync(categoryDto.Name)
            ) {
                return BadRequest($"Could not find the category.");
            }
            #endregion

            #region Database operations
            var category = await _categoryRepository.FindByIdAsync(categoryDto.Id.Value);
            _categoryRepository.Remove(category);
            var succeeded = await _categoryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
