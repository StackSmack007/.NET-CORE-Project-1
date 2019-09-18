namespace Junjuria.App.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Junjuria.App.ViewModels.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;

    public class ProductsController : BaseController
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public ProductsController(IRepository<Product> productsRepository, IRepository<Category> categoryRepository,IMapper mapper)
        {
            this.productsRepository = productsRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public IActionResult ProductsByCategory(int id)
        {
            var categoriesAll = CategoriesAll(id);
            var dtos = productsRepository.All().Where(x => categoriesAll.Contains(x.CategoryId))
                                              .ProjectTo<ProductMinorOutDto>(mapper.ConfigurationProvider).ToArray();

          //  return Json(products);
            ViewData["CategoryTitle"] = categoryRepository.All().FirstOrDefault(x => x.Id == id).Title;
            ViewData["CategoryInfo"] = categoryRepository.All().FirstOrDefault(x => x.Id == id).Description;
            return this.View("DisplayProducts", dtos);
        }

        private ICollection<int> CategoriesAll(int id)
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
