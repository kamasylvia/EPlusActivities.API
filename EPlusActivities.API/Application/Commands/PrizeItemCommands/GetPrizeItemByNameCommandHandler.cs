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
    public class GetPrizeItemByNameCommandHandler : BaseCommandHandler, IRequestHandler<GetPrizeItemByNameCommand, IEnumerable<PrizeItemDto>>
    {
        public GetPrizeItemByNameCommandHandler(UserManager<ApplicationUser> userManager, IPrizeItemRepository prizeItemRepository, INameExistsRepository<Brand> brandRepository, INameExistsRepository<Category> categoryRepository, IMapper mapper) : base(userManager, prizeItemRepository, brandRepository, categoryRepository, mapper)
        {
        }

        public async Task<IEnumerable<PrizeItemDto>> Handle(GetPrizeItemByNameCommand request, CancellationToken cancellationToken)
        {
            var prizeItems = await _prizeItemRepository.FindByNameAsync(request.Name);
            if (prizeItems.Count() <= 0)
            {
                throw new NotFoundException($"Could not find any prize item with name '{request.Name}' ");
            }
            
            return _mapper.Map<IEnumerable<PrizeItemDto>>(prizeItems);
        }
    }
}
