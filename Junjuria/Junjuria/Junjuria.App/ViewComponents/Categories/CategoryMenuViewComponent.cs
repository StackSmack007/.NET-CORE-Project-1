namespace Junjuria.App.ViewComponents.Categories
{
    using Junjuria.App.ViewComponents.DTO;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
   
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly IRepository<Category> categoryRepository;

        public CategoryMenuViewComponent(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoriesDtos = categoryRepository.All().Select(x => new CategoryOutDto
            {
                Id = x.Id,
                Title = x.Title,
                //Description = x.Description,
                OuterCategoryId = x.CategoryId,
                ProductsCount = x.Products.Count
            }).ToArray();
            EstimateTotalProducts(categoriesDtos);
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