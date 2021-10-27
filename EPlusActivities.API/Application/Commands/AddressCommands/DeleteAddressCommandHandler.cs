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
    public class DeleteAddressCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeleteAddressCommand>
    {
        public DeleteAddressCommandHandler(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) : base(userManager, addressRepository, mapper) { }

        public async Task<Unit> Handle(
            DeleteAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var address = await _addressRepository.FindByIdAsync(request.Id.Value);

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

            return new Unit();
        }
    }
}
