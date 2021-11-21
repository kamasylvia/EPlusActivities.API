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
    public class GetBrandListQueryHandler
        : BrandRequestHandlerBase,
          IRequestHandler<GetBrandListQuery, IEnumerable<BrandDto>>
    {
        public GetBrandListQueryHandler(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper
        ) : base(brandRepository, mapper) { }

        public async Task<IEnumerable<BrandDto>> Handle(
            GetBrandListQuery request,
            CancellationToken cancellationToken
        )
        {
            var brands = (await _brandRepository.FindAllAsync()).OrderBy(b => b.Name).ToList();

            var startIndex = (request.PageIndex - 1) * request.PageSize;
            var count = request.PageIndex * request.PageSize;

            if (brands.Count < startIndex)
            {
                throw new NotFoundException($"Could not find any brand.");
            }

            if (brands.Count - startIndex < count)
            {
                count = brands.Count - startIndex;
            }

            var result = brands.GetRange(startIndex, count);
            if (brands.Count <= 0)
            {
                throw new NotFoundException($"Could not found any brand.");
            }

            return _mapper.Map<IEnumerable<BrandDto>>(brands);
        }
    }
}
