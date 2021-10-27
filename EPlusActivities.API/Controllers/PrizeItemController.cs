using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 奖品 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class PrizeItemController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly IMapper _mapper;
        private readonly INameExistsRepository<Category> _categoryRepository;

        public PrizeItemController(
            UserManager<ApplicationUser> userManager,
            IPrizeItemRepository prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _brandRepository =
                brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository =
                categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        /// <summary>
        /// 通过奖品名获取奖品
        /// </summary>
        /// <param name="prizeItemDto"></param>
        /// <returns></returns>
        [HttpGet("name")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<PrizeItemDto>>> GetByNameAsync(
            [FromQuery] PrizeItemForGetByNameDto prizeItemDto
        )
        {
            var prizeItems = await _prizeItemRepository.FindByNameAsync(prizeItemDto.Name);
            return prizeItems.Count() > 0
              ? Ok(
                    _mapper.Map<IEnumerable<PrizeItemDto>>(
                        await _prizeItemRepository.FindByNameAsync(prizeItemDto.Name)
                    )
                )
              : NotFound($"Could not find any prize item with name '{prizeItemDto.Name}' ");
        }

        private async Task<IEnumerable<PrizeItem>> FindByIdListAsync(IEnumerable<Guid> ids) =>
            await ids.ToAsyncEnumerable()
                .SelectAwait(async id => await _prizeItemRepository.FindByIdAsync(id))
                .ToListAsync();

        /// <summary>
        /// 通过奖品 ID 列表获取奖品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeItemDto>> GetByIdsAsync(
            [FromQuery] PrizeItemForGetByIdsDto requestDto
        )
        {
            var ids = requestDto.Ids
                .Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries)
                .Select(s => Guid.Parse(s));
            var prizeItems = await FindByIdListAsync(ids);
            return prizeItems.Count() > 0
              ? Ok(_mapper.Map<IEnumerable<PrizeItemDto>>(prizeItems))
              : NotFound("Could not find any prizeItem.");
        }

        /// <summary>
        /// 通过奖品 ID 获取奖品
        /// </summary>
        /// <param name="prizeItemDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeItemDto>> GetByIdAsync(
            [FromQuery] PrizeItemForGetByIdDto prizeItemDto
        )
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id.Value);
            return prizeItem is null
              ? NotFound("Could not find the prizeItem.")
              : Ok(_mapper.Map<PrizeItemDto>(prizeItem));
        }

        /// <summary>
        /// 获取奖品列表
        /// </summary>
        /// <param name="prizeItemDto"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<PrizeItemDto>>> GetItemListAsync(
            [FromQuery] DtoForGetList prizeItemDto
        )
        {
            var prizeItemList = (await _prizeItemRepository.FindAllAsync())
                .OrderBy(item => item.Name)
                .ToList();
            var startIndex = (prizeItemDto.PageIndex - 1) * prizeItemDto.PageSize;
            var count = prizeItemDto.PageIndex * prizeItemDto.PageSize;

            if (prizeItemList.Count < startIndex)
            {
                return NotFound("Could not find any prizeItem.");
            }

            if (prizeItemList.Count - startIndex < count)
            {
                count = prizeItemList.Count - startIndex;
            }

            var result = prizeItemList.GetRange(startIndex, count);
            return result.Count > 0
              ? Ok(_mapper.Map<IEnumerable<PrizeItemDto>>(result))
              : NotFound("Could not find any prizeItem.");
        }

        /// <summary>
        /// 新建奖品
        /// </summary>
        /// <param name="prizeItemDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<PrizeItemDto>> CreateAsync(
            [FromBody] PrizeItemForCreateDto prizeItemDto
        )
        {
            #region New an entity
            var prizeItem = _mapper.Map<PrizeItem>(prizeItemDto);
            prizeItem.Brand = await GetBrandAsync(prizeItemDto.BrandName);
            prizeItem.Category = await GetCategoryAsync(prizeItemDto.CategoryName);
            #endregion

            #region Database operations
            await _prizeItemRepository.AddAsync(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();
            #endregion

            return succeeded
              ? Ok(_mapper.Map<PrizeItemDto>(prizeItem))
              : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 修改奖品
        /// </summary>
        /// <param name="prizeItemDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeItemForUpdateDto prizeItemDto)
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id.Value);

            #region Parameter validation
            if (prizeItem is null)
            {
                return BadRequest("The prize item is not existed");
            }
            ;
            #endregion

            #region New an entity
            prizeItem = _mapper.Map<PrizeItemForUpdateDto, PrizeItem>(prizeItemDto, prizeItem);
            prizeItem.Brand = await GetBrandAsync(prizeItemDto.BrandName);
            prizeItem.Category = await GetCategoryAsync(prizeItemDto.CategoryName);
            #endregion

            #region Database operations
            _prizeItemRepository.Update(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();
            #endregion

            return succeeded
              ? Ok()
              : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 删除奖品
        /// </summary>
        /// <param name="prizeItemDto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeItemForGetByIdDto prizeItemDto)
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id.Value);

            #region Parameter validation
            if (prizeItem is null)
            {
                return BadRequest("The prize item is not existed");
            }
            ;
            #endregion

            #region Database operations
            _prizeItemRepository.Remove(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();

            #endregion
            return succeeded
              ? Ok()
              : new InternalServerErrorObjectResult("Update database exception");
        }

        private async Task<Brand> GetBrandAsync(string brandName)
        {
            var brand = await _brandRepository.FindByNameAsync(brandName);
            if (brand is null)
            {
                brand = new Brand(brandName);
                await _brandRepository.AddAsync(brand);
            }
            return brand;
        }

        private async Task<Category> GetCategoryAsync(string categoryName)
        {
            var category = await _categoryRepository.FindByNameAsync(categoryName);
            if (category is null)
            {
                category = new Category(categoryName);
                await _categoryRepository.AddAsync(category);
            }
            return category;
        }
    }
}
