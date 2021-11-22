using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.AddressDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Queries.AddressQueries
{
    public class GetAddressQueryHandler
        : AddressRequestHandlerBase,
          IRequestHandler<GetAddressQuery, AddressDto>
    {
        public GetAddressQueryHandler(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(userManager, addressRepository, mapper) { }

        public async Task<AddressDto> Handle(
            GetAddressQuery request,
            CancellationToken cancellationToken
        )
        {
            var address = await _addressRepository.FindByIdAsync(request.Id.Value);
            if (address is null)
            {
                throw new NotFoundException($"Could not find the address with ID '{request.Id}'.");
            }

            return _mapper.Map<AddressDto>(address);
        }
    }
}
