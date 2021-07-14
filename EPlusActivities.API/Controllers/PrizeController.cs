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
    public class PrizeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByNameRepository<Prize> _prizeRepository;
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly IMapper _mapper;
        private readonly INameExistsRepository<Category> _categoryRepository;

        public PrizeController(
            UserManager<ApplicationUser> userManager,
            IFindByNameRepository<Prize> prizeRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _prizeRepository = prizeRepository
                ?? throw new ArgumentNullException(nameof(prizeRepository));
            _brandRepository = brandRepository
                ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository
                ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<PrizeDto>>> GetByNameAsync([FromBody] PrizeDto prizeDto) =>
            Ok(_mapper.Map<IEnumerable<PrizeDto>>(
                await _prizeRepository.FindByNameAsync(prizeDto.Name)));

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeDto>> GetByIdAsync([FromBody] PrizeDto prizeDto)
        {
            #region 参数验证
            if (prizeDto.Id == Guid.Empty)
            {
                return BadRequest("The prize ID could not be null");
            }
            #endregion

            var prize = await _prizeRepository.FindByIdAsync(prizeDto.Id);

            return prize is null ? NotFound("Could not find the prize.") : Ok(_mapper.Map<PrizeDto>(prize));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeDto>> AddPrizeAsync([FromBody] PrizeDto prizeDto)
        {
            #region 参数验证
            if (await _prizeRepository.ExistsAsync(prizeDto.Id))
            {
                return Conflict("This prize is already existed");
            }
            #endregion

            #region 数据库操作
            var prize = await GetPrizeAsync(prizeDto);
            await _prizeRepository.AddAsync(prize);
            var succeeded = await _prizeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<PrizeDto>(prize))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdatePrizeAsync([FromBody] PrizeDto prizeDto)
        {
            #region 参数验证
            if (prizeDto.Id == Guid.Empty)
            {
                return BadRequest("The prize Id could not be null");
            }

            if (!await _prizeRepository.ExistsAsync(prizeDto.Id))
            {
                return BadRequest("The prize is not existed");
            };
            #endregion

            var prize = await GetPrizeAsync(prizeDto);

            #region 数据库操作
            _prizeRepository.Update(prize);
            var succeeded = await _prizeRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeletePrizeAsync([FromBody] PrizeDto prizeDto)
        {
            #region 参数验证
            if (prizeDto.Id == Guid.Empty)
            {
                return BadRequest("The prize Id could not be null");
            }

            if (!await _prizeRepository.ExistsAsync(prizeDto.Id))
            {
                return BadRequest("The prize is not existed");
            };
            #endregion

            #region 数据库操作
            var prize = await _prizeRepository.FindByIdAsync(prizeDto.Id);
            _prizeRepository.Remove(prize);
            var succeeded = await _prizeRepository.SaveAsync();

            #endregion
            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        private async Task<Prize> GetPrizeAsync(PrizeDto prizeDto)
        {
            #region 新建奖品
            var prize = _mapper.Map<Prize>(prizeDto);
            prize.Brand = await GetBrandAsync(prizeDto);
            prize.Category = await GetCategoryAsync(prizeDto);
            #endregion
            return prize;
        }

        private async Task<Brand> GetBrandAsync(PrizeDto prizeDto)
        {
            #region Get brand
            var brand = await _brandRepository.FindByNameAsync(prizeDto.BrandName);
            if (brand is null)
            {
                brand = new Brand(prizeDto.BrandName);
                await _brandRepository.AddAsync(brand);
            }
            #endregion
            return brand;
        }

        private async Task<Category> GetCategoryAsync(PrizeDto prizeDto)
        {
            #region Get Category
            var category = await _categoryRepository.FindByNameAsync(prizeDto.CategoryName);
            if (category is null)
            {
                category = new Category(prizeDto.CategoryName);
                await _categoryRepository.AddAsync(category);
            }
            #endregion
            return category;
        }
    }
}
