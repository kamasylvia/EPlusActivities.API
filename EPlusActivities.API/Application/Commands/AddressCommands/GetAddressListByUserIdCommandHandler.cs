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
    public class GetAddressListByUserIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetAddressListByUserIdCommand, IEnumerable<AddressDto>>
    {
        public GetAddressListByUserIdCommandHandler(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(userManager, addressRepository, mapper) { }

        public async Task<IEnumerable<AddressDto>> Handle(
            GetAddressListByUserIdCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            var addresses = await _addressRepository.FindByParentIdAsync(request.UserId.Value);
            if (addresses.Count() <= 0)
            {
                throw new NotFoundException(
                    $"Could not find any addresses with the specified user '{request.UserId.Value}'"
                );
            }

            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }
    }
}
