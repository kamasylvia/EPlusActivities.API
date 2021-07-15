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
    public class CategoryController : Controller
    {

        private readonly INameExistsRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository
                ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<CategoryDto>> GetAsync([FromBody] CategoryDto categoryDto)
        {
            #region Parameter validation
            if (categoryDto.Id == Guid.Empty)
            {
                return BadRequest("The ID could not be null.");
            }
            #endregion
            
            var category = await _categoryRepository.FindByIdAsync(categoryDto.Id)
                           ?? await _categoryRepository.FindByNameAsync(categoryDto.Name);

            return category is null
                ? NotFound($"Could not find the category '{categoryDto.Name}'")
                : Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<CategoryDto>> CreateAsync([FromBody] CategoryDto categoryDto)
        {
            #region Parameter validation
            if (await _categoryRepository.ExistsAsync(categoryDto.Name))
            {
                return Conflict($"The category '{categoryDto.Name}' is already existed.");
            }
            #endregion

            #region New an entity
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = Guid.NewGuid();
            #endregion

            #region Database operations
            await _categoryRepository.AddAsync(category);
            var succeeded = await _categoryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<CategoryDto>(category))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] CategoryDto categoryDto)
        {
            #region Parameter validation
            if (!await _categoryRepository.ExistsAsync(categoryDto.Id))
            {
                return NotFound($"Could not find the category with ID '{categoryDto.Id}'");
            }
            #endregion

            #region Database operations
            var category = _mapper.Map<Category>(categoryDto);
            _categoryRepository.Update(category);
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
            if (categoryDto.Id == Guid.Empty)
            {
                return BadRequest("The ID is required");
            }

            if (!await _categoryRepository.ExistsAsync(categoryDto.Id)
                || !await _categoryRepository.ExistsAsync(categoryDto.Name))
            {
                return BadRequest($"Could not find the category '{categoryDto.Name}'");
            }
            #endregion

            #region Database operations
            var category = _mapper.Map<Category>(categoryDto);
            _categoryRepository.Remove(category);
            var succeeded = await _categoryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
