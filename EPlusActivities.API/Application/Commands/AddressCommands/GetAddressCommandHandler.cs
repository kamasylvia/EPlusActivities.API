using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.AddressDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class GetAddressCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetAddressCommand, AddressDto>
    {
        public GetAddressCommandHandler(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(userManager, addressRepository, mapper) { }

        public async Task<AddressDto> Handle(
            GetAddressCommand request,
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
