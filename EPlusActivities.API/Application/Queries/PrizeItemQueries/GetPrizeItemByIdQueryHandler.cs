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

namespace EPlusActivities.API.Application.Queries.PrizeItemQueries
{
    public class GetPrizeItemByIdQueryHandler
        : PrizeItemRequestHandlerBase,
          IRequestHandler<GetPrizeItemByIdQuery, PrizeItemDto>
    {
        public GetPrizeItemByIdQueryHandler(
            UserManager<ApplicationUser> userManager,
            IPrizeItemRepository prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(userManager, prizeItemRepository, brandRepository, categoryRepository, mapper) { }

        public async Task<PrizeItemDto> Handle(
            GetPrizeItemByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(request.Id.Value);
            if (prizeItem is null)
            {
                throw new NotFoundException("Could not find the prizeItem.");
            }
            return _mapper.Map<PrizeItemDto>(prizeItem);
        }
    }
}
