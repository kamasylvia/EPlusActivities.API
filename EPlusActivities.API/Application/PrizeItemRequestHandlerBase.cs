using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application
{
    public abstract class PrizeItemRequestHandlerBase
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IPrizeItemRepository _prizeItemRepository;
        protected readonly INameExistsRepository<Brand> _brandRepository;
        protected readonly IMapper _mapper;
        protected readonly INameExistsRepository<Category> _categoryRepository;
        public PrizeItemRequestHandlerBase(
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

        protected async Task<IEnumerable<PrizeItem>> FindByIdListAsync(IEnumerable<Guid> ids) =>
       await ids.ToAsyncEnumerable()
           .SelectAwait(async id => await _prizeItemRepository.FindByIdAsync(id))
           .ToListAsync();
    }
}
