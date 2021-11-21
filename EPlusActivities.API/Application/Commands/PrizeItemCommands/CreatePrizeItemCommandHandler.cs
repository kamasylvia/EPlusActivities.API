using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class CreatePrizeItemCommandHandler
        : PrizeItemRequestHandlerBase,
          IRequestHandler<CreatePrizeItemCommand, PrizeItemDto>
    {
        public CreatePrizeItemCommandHandler(
            UserManager<ApplicationUser> userManager,
            IPrizeItemRepository prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(userManager, prizeItemRepository, brandRepository, categoryRepository, mapper) { }

        public async Task<PrizeItemDto> Handle(
            CreatePrizeItemCommand request,
            CancellationToken cancellationToken
        )
        {
            #region New an entity
            var prizeItem = _mapper.Map<PrizeItem>(request);
            if (!string.IsNullOrEmpty(request.BrandName))
            {
                prizeItem.Brand = await GetBrandAsync(request.BrandName);
            }
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                prizeItem.Category = await GetCategoryAsync(request.CategoryName);
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
    }
}
