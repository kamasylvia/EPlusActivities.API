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
    public class GetPrizeItemGroupCommandHandler : BaseCommandHandler, IRequestHandler<GetPrizeItemGroupCommand, IEnumerable<PrizeItemDto>>
    {
        public GetPrizeItemGroupCommandHandler(UserManager<ApplicationUser> userManager, IPrizeItemRepository prizeItemRepository, INameExistsRepository<Brand> brandRepository, INameExistsRepository<Category> categoryRepository, IMapper mapper) : base(userManager, prizeItemRepository, brandRepository, categoryRepository, mapper)
        {
        }

        public async Task<IEnumerable<PrizeItemDto>> Handle(GetPrizeItemGroupCommand request, CancellationToken cancellationToken)
        {
            var ids = request.Ids
                .Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries)
                .Select(s => Guid.Parse(s));
            var prizeItems = await FindByIdListAsync(ids);
            if (prizeItems.Count()<=0)
            {
                throw new NotFoundException("Could not find any prizeItem.");
            }
            return _mapper.Map<IEnumerable<PrizeItemDto>>(prizeItems);
        }
    }
}
