using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.BrandCommands;
using EPlusActivities.API.Dtos.BrandDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Application.Actors.BrandActors
{
    public class BrandActor : Actor, IBrandActor
    {
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly IMapper _mapper;

        public BrandActor(ActorHost host, INameExistsRepository<Brand> brandRepository, IMapper mapper) : base(host)
        {
            _brandRepository =
                brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BrandDto> CreateBrand(CreateBrandCommand command)
        {
            #region Parameter validation
            if (await _brandRepository.ExistsAsync(command.Name))
            {
                throw new ConflictException($"The brand {command.Name} is already existed");
            }
            #endregion

            #region New an entity
            var brand = _mapper.Map<Brand>(command);
            #endregion

            #region Database operations
            await _brandRepository.AddAsync(brand);
            if (!await _brandRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<BrandDto>(brand);

        }

        public async Task DeleteBrand(DeleteBrandCommand command)
        {
            #region Parameter validation
            if (!await _brandRepository.ExistsAsync(command.Id.Value))
            {
                throw new NotFoundException($"Could not find the brand.");
            }
            #endregion

            #region Database operations
            var brand = await _brandRepository.FindByIdAsync(command.Id.Value);
            _brandRepository.Remove(brand);
            if (!await _brandRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task UpdateBrandName(UpdateBrandNameCommand command)
        {
            #region Parameter validation
            if (!await _brandRepository.ExistsAsync(command.Id.Value))
            {
                throw new NotFoundException($"Could not find the brand.");
            }
            #endregion

            #region Database operations
            var brand = await _brandRepository.FindByIdAsync(command.Id.Value);
            brand = _mapper.Map<UpdateBrandNameCommand, Brand>(command, brand);
            _brandRepository.Update(brand);
            if (!await _brandRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
