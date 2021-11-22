using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.AddressCommands;
using EPlusActivities.API.Dtos.AddressDtos;

namespace EPlusActivities.API.Application.Actors.AddressActors
{
    public interface IAddressActor : IActor
    {
        Task<AddressDto> CreateAddress(CreateAddressCommand command);
        Task DeleteAddress(DeleteAddressCommand command);
        Task UpdateAddress(UpdateAddressCommand command);
    }
}
