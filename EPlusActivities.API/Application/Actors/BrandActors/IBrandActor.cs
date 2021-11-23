using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.BrandCommands;
using EPlusActivities.API.Dtos.BrandDtos;

namespace EPlusActivities.API.Application.Actors.BrandActors
{
    public interface IBrandActor : IActor
    {
        Task<BrandDto> CreateBrand(CreateBrandCommand command);
        Task DeleteBrand(DeleteBrandCommand command);
        Task UpdateBrandName(UpdateBrandCommand command);
    }
}
