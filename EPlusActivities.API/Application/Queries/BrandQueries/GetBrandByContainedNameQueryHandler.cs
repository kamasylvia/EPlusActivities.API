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
    public class GetBrandByContainedNameQueryHandler
        : BrandRequestHandlerBase,
          IRequestHandler<GetBrandByContainedNameQuery, IEnumerable<BrandDto>>
    {
        public GetBrandByContainedNameQueryHandler(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper
        ) : base(brandRepository, mapper) { }

        public async Task<IEnumerable<BrandDto>> Handle(
            GetBrandByContainedNameQuery request,
            CancellationToken cancellationToken
        )
        {
            var brands = await _brandRepository.FindByContainedNameAsync(request.Keyword);
            if (brands.Count() <= 0)
            {
                throw new NotFoundException(
                    $"Could not find any brand with name '{request.Keyword}'"
                );
            }
            return _mapper.Map<IEnumerable<BrandDto>>(brands);
        }
    }
}
