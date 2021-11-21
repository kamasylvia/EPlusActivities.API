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

namespace EPlusActivities.API.Application.Queries.BrandQueries
{
    public class GetBrandByIdQueryHandler
        : BrandRequestHandlerBase,
          IRequestHandler<GetBrandByIdQuery, BrandDto>
    {
        public GetBrandByIdQueryHandler(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper
        ) : base(brandRepository, mapper) { }

        public async Task<BrandDto> Handle(
            GetBrandByIdQuery request,
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
