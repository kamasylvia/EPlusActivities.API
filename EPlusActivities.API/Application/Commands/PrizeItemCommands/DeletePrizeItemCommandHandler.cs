using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class DeletePrizeItemCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeletePrizeItemCommand>
    {
        public DeletePrizeItemCommandHandler(
            UserManager<ApplicationUser> userManager,
            IPrizeItemRepository prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        ) : base(userManager, prizeItemRepository, brandRepository, categoryRepository, mapper) { }

        public async Task<Unit> Handle(
            DeletePrizeItemCommand request,
            CancellationToken cancellationToken
        )
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(request.Id.Value);

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

            return Unit.Value;
        }
    }
}
