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

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class UpdateBrandNameCommandHandler
        : BrandRequestHandlerBase,
          IRequestHandler<UpdateBrandNameCommand>
    {
        public UpdateBrandNameCommandHandler(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper
        ) : base(brandRepository, mapper) { }

        public async Task<Unit> Handle(
            UpdateBrandNameCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (!await _brandRepository.ExistsAsync(request.Id.Value))
            {
                throw new NotFoundException($"Could not find the brand.");
            }
            #endregion

            #region Database operations
            var brand = await _brandRepository.FindByIdAsync(request.Id.Value);
            brand = _mapper.Map<UpdateBrandNameCommand, Brand>(request, brand);
            _brandRepository.Update(brand);
            if (!await _brandRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
