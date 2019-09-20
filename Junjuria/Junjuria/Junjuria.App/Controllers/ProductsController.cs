namespace Junjuria.App.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Junjuria.App.ViewModels.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class ProductsController : BaseController
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public ProductsController(IRepository<Product> productsRepository, IRepository<Category> categoryRepository, IMapper mapper)
        {
            this.productsRepository = productsRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public IActionResult ProductsByCategory(int id)
        {
            var categoriesAll = AllSubCategories(id);
            var dtos = productsRepository.All().Where(x => categoriesAll.Contains(x.CategoryId))
                                              .ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider).ToArray();

            //  return Json(products);
            var category = categoryRepository.All().FirstOrDefault(x => x.Id == id);
            ViewData["SubHead1"] = new string[] { "View products of category - ", $"{category.Title}" };
            ViewData["SubHead2"] = new string[] { "Category info: ", $"{category.Description}" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult OnSale()
        {
            var dtos = productsRepository.All().Where(x => x.Discount > 0 && x.Quantity != 0).OrderByDescending(x => x.Discount)
                                              .ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider).ToArray();
            ViewData["SubHead1"] = new string[] { "View products on sale:" };
            //ViewData["SubHead2"] = new string[] { "Category info: ", $"{category.Description}" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult MostPurchased()
        {
            var dtos = productsRepository.All().OrderByDescending(x => x.ProductOrders.Count)
                                              .ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider).Take(10).ToArray();
            ViewData["SubHead1"] = new string[] { "Top 10 most purchased products" };
            //ViewData["SubHead2"] = new string[] { "Category info: ", $"{category.Description}" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult MostCommented()
        {
            var dtos = productsRepository.All().OrderByDescending(x => x.ProductComments.Count)
                                              .ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider).Take(10).ToArray();
            ViewData["SubHead1"] = new string[] { "Top 10 most commented products" };
            return this.View("DisplayProducts", dtos);
        }
        public IActionResult BestRated()
        {
            var dtos = productsRepository.All().OrderByDescending(x => x.Votes.Count).ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider)
                .OrderByDescending(x => x.Grade).Take(10).ToArray();
            ViewData["SubHead1"] = new string[] { "Top 10 best and most rated products" };
            return this.View("DisplayProducts", dtos);
        }
        public IActionResult All()
        {
            var dtos = productsRepository.All().ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider)
                                               .OrderByDescending(x => x.IsAvailable)
                                               .ThenBy(x => x.Price).ToArray();
            ViewData["SubHead1"] = new string[] { "All of our products:" };
            return this.View("DisplayProducts", dtos);
        }

        private ICollection<int> AllSubCategories(int id)
        {
            HashSet<int> ids = new HashSet<int>();
            ids.Add(id);
            var idPool = categoryRepository.All().Where(x => x.CategoryId != null).Select(x => new
            {
                x.Id,
                x.CategoryId
            }).ToArray();

            while (true)
            {
                int collectedBefore = ids.Count();
                foreach (var idPack in idPool)
                {
                    if (ids.Contains(idPack.CategoryId.Value) && !ids.Contains(idPack.Id))
                    {
                        ids.Add(idPack.Id);
                    }
                }
                if (collectedBefore == ids.Count()) break;
            }
            return ids;
        }
    }
}
