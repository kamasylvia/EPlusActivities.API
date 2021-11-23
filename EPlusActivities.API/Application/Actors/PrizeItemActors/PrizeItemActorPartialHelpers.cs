using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Application.Actors.PrizeItemActors
{
    public partial class PrizeItemActor
    {
        private async Task<Brand> GetBrandAsync(string brandName)
        {
            var brand = await _brandRepository.FindByNameAsync(brandName);
            if (brand is null)
            {
                brand = new Brand(brandName);
                await _brandRepository.AddAsync(brand);
            }
            return brand;
        }

        private async Task<Category> GetCategoryAsync(string categoryName)
        {
            var category = await _categoryRepository.FindByNameAsync(categoryName);
            if (category is null)
            {
                category = new Category(categoryName);
                await _categoryRepository.AddAsync(category);
            }
            return category;
        }
    }
}
