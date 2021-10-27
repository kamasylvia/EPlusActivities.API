using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public abstract class BaseCommandHandler
    {
        protected readonly INameExistsRepository<Brand> _brandRepository;
        protected readonly IMapper _mapper;

        public BaseCommandHandler(INameExistsRepository<Brand> brandRepository, IMapper mapper)
        {
            _brandRepository =
                brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
