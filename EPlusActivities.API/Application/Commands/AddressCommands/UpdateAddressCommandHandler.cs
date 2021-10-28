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

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class UpdateAddressCommandHandler
        : BaseCommandHandler,
          IRequestHandler<UpdateAddressCommand>
    {
        public UpdateAddressCommandHandler(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(userManager, addressRepository, mapper) { }

        public async Task<Unit> Handle(
            UpdateAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var address = await _addressRepository.FindByIdAsync(request.Id.Value);

            #region Parameter validation
            if (address is null)
            {
                throw new NotFoundException("Could not find the address.");
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
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

            _addressRepository.Update(_mapper.Map<UpdateAddressCommand, Address>(request, address));
            if (!await _addressRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return Unit.Value;
        }
    }
}
