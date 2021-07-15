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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class PrizeItemController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByNameRepository<PrizeItem> _prizeItemRepository;
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly IMapper _mapper;
        private readonly INameExistsRepository<Category> _categoryRepository;

        public PrizeItemController(
            UserManager<ApplicationUser> userManager,
            IFindByNameRepository<PrizeItem> prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _prizeItemRepository = prizeItemRepository
                ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _brandRepository = brandRepository
                ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository
                ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<PrizeItemDto>>> GetByNameAsync([FromBody] PrizeItemDto prizeItemDto) =>
            Ok(_mapper.Map<IEnumerable<PrizeItemDto>>(
                await _prizeItemRepository.FindByNameAsync(prizeItemDto.Name)));

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeItemDto>> GetAsync([FromBody] PrizeItemDto prizeItemDto)
        {
            #region Parameter validation
            if (prizeItemDto.Id == Guid.Empty)
            {
                return BadRequest("The prize item ID could not be null");
            }
            #endregion

            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id);

            return prizeItem is null ? NotFound("Could not find the prizeItem.") : Ok(_mapper.Map<PrizeItemDto>(prizeItem));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeItemDto>> CreateAsync([FromBody] PrizeItemDto prizeItemDto)
        {
            #region Parameter validation
            if (await _prizeItemRepository.ExistsAsync(prizeItemDto.Id))
            {
                return Conflict("This prize item is already existed");
            }
            #endregion

            #region Database operations
            var prizeItem = await GetPrizeItemAsync(prizeItemDto);
            await _prizeItemRepository.AddAsync(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<PrizeItemDto>(prizeItem))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeItemDto prizeItemDto)
        {
            #region Parameter validation
            if (prizeItemDto.Id == Guid.Empty)
            {
                return BadRequest("The prize item Id could not be null");
            }

            if (!await _prizeItemRepository.ExistsAsync(prizeItemDto.Id))
            {
                return BadRequest("The prize item is not existed");
            };
            #endregion

            var prizeItem = await GetPrizeItemAsync(prizeItemDto);

            #region Database operations
            _prizeItemRepository.Update(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeItemDto prizeItemDto)
        {
            #region Parameter validation
            if (prizeItemDto.Id == Guid.Empty)
            {
                return BadRequest("The prize item Id could not be null");
            }

            if (!await _prizeItemRepository.ExistsAsync(prizeItemDto.Id))
            {
                return BadRequest("The prize item is not existed");
            };
            #endregion

            #region Database operations
            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id);
            _prizeItemRepository.Remove(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();

            #endregion
            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        private async Task<PrizeItem> GetPrizeItemAsync(PrizeItemDto prizeItemDto)
        {
            #region New an entity
            var prizeItem = _mapper.Map<PrizeItem>(prizeItemDto);
            prizeItem.Brand = await GetBrandAsync(prizeItemDto);
            prizeItem.Category = await GetCategoryAsync(prizeItemDto);
            #endregion
            return prizeItem;
        }

        private async Task<Brand> GetBrandAsync(PrizeItemDto prizeItemDto)
        {
            #region Get brand
            var brand = await _brandRepository.FindByNameAsync(prizeItemDto.BrandName);
            if (brand is null)
            {
                brand = new Brand(prizeItemDto.BrandName);
                await _brandRepository.AddAsync(brand);
            }
            #endregion
            return brand;
        }

        private async Task<Category> GetCategoryAsync(PrizeItemDto prizeItemDto)
        {
            #region Get Category
            var category = await _categoryRepository.FindByNameAsync(prizeItemDto.CategoryName);
            if (category is null)
            {
                category = new Category(prizeItemDto.CategoryName);
                await _categoryRepository.AddAsync(category);
            }
            #endregion
            return category;
        }
    }
}
