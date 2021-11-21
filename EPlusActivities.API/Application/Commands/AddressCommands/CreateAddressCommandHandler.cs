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
    public class CreateAddressCommandHandler
        : AddressRequestHandlerBase,
          IRequestHandler<CreateAddressCommand, AddressDto>
    {
        public CreateAddressCommandHandler(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(userManager, addressRepository, mapper) { }

        public async Task<AddressDto> Handle(
            CreateAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var oldAddresses = await _addressRepository.FindByParentIdAsync(user.Id);
            if (oldAddresses?.Count() >= 5)
            {
                throw new BadRequestException("Could not add more than 5 addresses.");
            }
            #endregion

            #region Database operations
            if (request.IsDefault)
            {
                var addresses = await _addressRepository.FindByParentIdAsync(request.UserId.Value);
                if (addresses.Count() > 0)
                {
                    var oldDefaultAddress = addresses.Single(x => x.IsDefault);
                    oldDefaultAddress.IsDefault = false;
                    _addressRepository.Update(oldDefaultAddress);
                }
            }

            var address = _mapper.Map<Address>(request);
            await _addressRepository.AddAsync(address);
            if (!await _addressRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<AddressDto>(address);
        }
    }
}
