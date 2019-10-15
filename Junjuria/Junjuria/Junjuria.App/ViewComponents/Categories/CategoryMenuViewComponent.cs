namespace Junjuria.App.ViewComponents.Categories
{
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Junjuria.Infrastructure.Models;
using Junjuria.App.ViewComponents.DTO;
using Junjuria.Services.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
    using Junjuria.Common;

    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly IRepository<Category> categoryRepository;
        private MemoryCacheEntryOptions cacheOptions;
        private readonly IMemoryCache memoryCache;


        public CategoryMenuViewComponent(IRepository<Category> categoryRepository, IMemoryCache memoryCache)
        {
            this.categoryRepository = categoryRepository;
            this.memoryCache = memoryCache;
            this.cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(2))
                .SetPriority(CacheItemPriority.Normal);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            CategoryOutDto[] categoriesDtos;
            if (!memoryCache.TryGetValue(GlobalConstants.CasheCategoriesInButtonName, out categoriesDtos))
            {
                categoriesDtos = categoryRepository.All().Select(x => new CategoryOutDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    OuterCategoryId = x.CategoryId,
                    ProductsCount = x.Products.Where(p => !p.IsDeleted).Count()
                }).ToArray();
                EstimateTotalProducts(categoriesDtos);
                memoryCache.Set(GlobalConstants.CasheCategoriesInButtonName, categoriesDtos, cacheOptions);
            }
            return await Task.Run(() => this.View(categoriesDtos));
        }

        private void EstimateTotalProducts(CategoryOutDto[] categories)
        {
            List<CategoryOutDto> categoriesRemaining = categories.ToList();
            List<int> bottomlessCategories = categoriesRemaining.Where(x => categories.All(c => c.OuterCategoryId != x.Id)).Select(x => x.Id).ToList();
            while (categoriesRemaining.Count > 0)
            {
                foreach (var id in bottomlessCategories)
                {
                    CategoryOutDto source = categoriesRemaining.First(x => x.Id == id);
                    if (source.OuterCategoryId is null) continue;
                    CategoryOutDto target = categoriesRemaining.First(x => x.Id == source.OuterCategoryId);
                    target.SubProductsCount += source.ProductsCount + source.SubProductsCount;
                }
                categoriesRemaining.RemoveAll(x => bottomlessCategories.Contains(x.Id));
                bottomlessCategories = categoriesRemaining.Where(x => categoriesRemaining.All(c => c.OuterCategoryId != x.Id)).Select(x => x.Id).ToList();
            }
        }
    }
}