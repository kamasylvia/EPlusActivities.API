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
    public class GetBrandByIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetBrandByIdCommand, BrandDto>
    {
        public GetBrandByIdCommandHandler(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper
        ) : base(brandRepository, mapper) { }

        public async Task<BrandDto> Handle(
            GetBrandByIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var brand = await _brandRepository.FindByIdAsync(request.Id.Value);
            if (brand is null)
            {
                throw new NotFoundException("Could not find the brand.");
            }
            return _mapper.Map<BrandDto>(brand);
        }
    }
}
