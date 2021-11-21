using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application
{
    public abstract class AddressRequestHandlerBase
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IFindByParentIdRepository<Address> _addressRepository;
        protected readonly IMapper _mapper;

        protected AddressRequestHandlerBase(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _addressRepository =
                addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
