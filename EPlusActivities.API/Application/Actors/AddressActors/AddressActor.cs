using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.AddressCommands;
using EPlusActivities.API.Dtos.AddressDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.AddressActors
{
    public class AddressActor : Actor, IAddressActor
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByParentIdRepository<Address> _addressRepository;
        private readonly IMapper _mapper;

        public AddressActor(
            ActorHost host,
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(host)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _addressRepository =
                addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressDto> CreateAddress(CreateAddressCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
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
            if (command.IsDefault)
            {
                var addresses = await _addressRepository.FindByParentIdAsync(command.UserId.Value);
                if (addresses.Count() > 0)
                {
                    var oldDefaultAddress = addresses.Single(x => x.IsDefault);
                    oldDefaultAddress.IsDefault = false;
                    _addressRepository.Update(oldDefaultAddress);
                }
            }

            var address = _mapper.Map<Address>(command);
            await _addressRepository.AddAsync(address);
            if (!await _addressRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<AddressDto>(address);
        }

        public async Task DeleteAddress(DeleteAddressCommand command)
        {
            var address = await _addressRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (address is null)
            {
                throw new NotFoundException("Could not find the address.");
            }
            #endregion

            #region Database operations
            _addressRepository.Remove(address);
            if (!await _addressRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task UpdateAddress(UpdateAddressCommand command)
        {
            var address = await _addressRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (address is null)
            {
                throw new NotFoundException("Could not find the address.");
            }

            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            #region Database operations
            if (command.IsDefault)
            {
                var addresses = await _addressRepository.FindByParentIdAsync(command.UserId.Value);
                if (addresses.Count() > 0)
                {
                    var oldDefaultAddress = addresses.Single(x => x.IsDefault);
                    oldDefaultAddress.IsDefault = false;
                    _addressRepository.Update(oldDefaultAddress);
                }
            }

            _addressRepository.Update(_mapper.Map<UpdateAddressCommand, Address>(command, address));
            if (!await _addressRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
