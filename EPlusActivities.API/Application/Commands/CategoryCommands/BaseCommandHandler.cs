using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public abstract class BaseCommandHandler
    {
        protected readonly INameExistsRepository<Category> _categoryRepository;
        protected readonly IMapper _mapper;

        public BaseCommandHandler(
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper
        )
        {
            _categoryRepository =
                categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
