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
    public class GetPrizeItemListCommandHandler : BaseCommandHandler, IRequestHandler<GetPrizeItemListCommand, IEnumerable<PrizeItemDto>>
    {
        public GetPrizeItemListCommandHandler(UserManager<ApplicationUser> userManager, IPrizeItemRepository prizeItemRepository, INameExistsRepository<Brand> brandRepository, INameExistsRepository<Category> categoryRepository, IMapper mapper) : base(userManager, prizeItemRepository, brandRepository, categoryRepository, mapper)
        {
        }

        public async Task<IEnumerable<PrizeItemDto>> Handle(GetPrizeItemListCommand request, CancellationToken cancellationToken)
        {
            var prizeItemList = (await _prizeItemRepository.FindAllAsync())
                .OrderBy(item => item.Name)
                .ToList();
            var startIndex = (request.PageIndex - 1) * request.PageSize;
            var count = request.PageIndex * request.PageSize;

            if (prizeItemList.Count < startIndex)
            {
                throw new NotFoundException("Could not find any prizeItem.");
            }

            if (prizeItemList.Count - startIndex < count)
            {
                count = prizeItemList.Count - startIndex;
            }

            var result = prizeItemList.GetRange(startIndex, count);

            if (result.Count<=0)
            {
                throw new NotFoundException("Could not find any prizeItem.");
            }
            
            return _mapper.Map<IEnumerable<PrizeItemDto>>(result);
        }
    }
}
