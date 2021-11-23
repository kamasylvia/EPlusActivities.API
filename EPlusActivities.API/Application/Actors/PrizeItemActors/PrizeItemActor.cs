using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.PrizeItemCommands;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.PrizeItemActors
{
    public partial class PrizeItemActor : Actor, IPrizeItemActor
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly INameExistsRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public PrizeItemActor(ActorHost host,
            UserManager<ApplicationUser> userManager,
            IPrizeItemRepository prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        
        ) : base(host)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _prizeItemRepository = prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PrizeItemDto> CreatePrizeItem(CreatePrizeItemCommand command)
        {
            #region New an entity
            var prizeItem = _mapper.Map<PrizeItem>(command);
            if (!string.IsNullOrEmpty(command.BrandName))
            {
                prizeItem.Brand = await GetBrandAsync(command.BrandName);
            }
            if (!string.IsNullOrEmpty(command.CategoryName))
            {
                prizeItem.Category = await GetCategoryAsync(command.CategoryName);
            }
            #endregion

            #region Database operations
            await _prizeItemRepository.AddAsync(prizeItem);
            if (!await _prizeItemRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            var result = _mapper.Map<PrizeItemDto>(prizeItem);
            result.BrandName = prizeItem?.Brand?.Name;
            result.CategoryName = prizeItem?.Category?.Name;
            
            return result;
        }

        public async Task DeletePrizeItem(DeletePrizeItemCommand command)
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (prizeItem is null)
            {
                throw new BadRequestException("The prize item is not existed");
            }
            ;
            #endregion

            #region Database operations
            _prizeItemRepository.Remove(prizeItem);
            if (!await _prizeItemRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task UpdatePrizeItem(UpdatePrizeItemCommand command)
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (prizeItem is null)
            {
                throw new BadRequestException("The prize item is not existed");
            }
            ;
            #endregion

            #region New an entity
            prizeItem = _mapper.Map<UpdatePrizeItemCommand, PrizeItem>(command, prizeItem);
            prizeItem.Brand = await GetBrandAsync(command.BrandName);
            prizeItem.Category = await GetCategoryAsync(command.CategoryName);
            #endregion

            #region Database operations
            _prizeItemRepository.Update(prizeItem);
            if (!await _prizeItemRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
