using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.BrandDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class CreateBrandCommandHandler
        : BrandRequestHandlerBase,
          IRequestHandler<CreateBrandCommand, BrandDto>
    {
        public CreateBrandCommandHandler(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper
        ) : base(brandRepository, mapper) { }

        public async Task<BrandDto> Handle(
            CreateBrandCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            if (await _brandRepository.ExistsAsync(request.Name))
            {
                throw new ConflictException($"The brand {request.Name} is already existed");
            }
            #endregion

            #region New an entity
            var brand = _mapper.Map<Brand>(request);
            #endregion

            #region Database operations
            await _brandRepository.AddAsync(brand);
            if (!await _brandRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<BrandDto>(brand);
        }
    }
}
