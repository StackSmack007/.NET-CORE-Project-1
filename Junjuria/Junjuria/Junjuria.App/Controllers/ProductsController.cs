namespace Junjuria.App.Controllers
{
    using Junjuria.Common;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using X.PagedList;

    public class ProductsController : BaseController
    {
        private readonly IProductsService productsService;
        private readonly ICategoryService categoriesService;

        public ProductsController(IProductsService productsService, ICategoryService categoriesService)
        {
            this.productsService = productsService;
            this.categoriesService = categoriesService;
        }

        public IActionResult ProductsByCategory(int id)
        {
            ICollection<int> desiredCategories = categoriesService.GetSubcategoriesOfCagetoryId(id);
            var dtos = productsService.GetProductsByCategories(desiredCategories).ToArray();
            Category categoryName = categoriesService.GetById(id);
            ViewData["SubHead1"] = new string[] { "View products of category - ", $"{categoryName.Title}" };
            ViewData["SubHead2"] = new string[] { "Category info: ", $"{categoryName.Description}" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult OnSale()
        {
            var dtos = productsService.GetOnSale().ToArray();
            ViewData["SubHead1"] = new string[] { "View products on sale:" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult MostPurchased()
        {
            var dtos = productsService.GetMostPurchased(GlobalConstants.MostPurchasedTotalCount).ToArray();
            ViewData["SubHead1"] = new string[] { $"Top {GlobalConstants.MostPurchasedTotalCount} most purchased products" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult MostCommented()
        {
            var dtos = productsService.GetMostCommented(GlobalConstants.MostCommentedTotalCount).ToArray();
            ViewData["SubHead1"] = new string[] { $"Top {GlobalConstants.MostCommentedTotalCount} most commented products" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult BestRated()
        {
            var dtos = productsService.GetMostRated(GlobalConstants.MostRatedTotalCount).ToArray();

            ViewData["SubHead1"] = new string[] { $"Top {GlobalConstants.MostRatedTotalCount} best and most rated products" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult All(int? pageNum)
        {
            ViewBag.PageNavigation = productsService.GetAll().Count() > GlobalConstants.MaximumCountOfAllProductsOnSinglePage;
            var dtos = productsService.GetAll().ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfAllProductsOnSinglePage);
            ViewData["SubHead1"] = new string[] { "All of our products:" };
            return this.View("DisplayProducts", dtos);
        }

        public IActionResult Details(int id)
        {
            var product = productsService.GetDetails(id);
            return this.View(product);
            // return Json(productsService.GetDetails(id),new Newtonsoft.Json.JsonSerializerSettings { Formatting = Newtonsoft.Json.Formatting.Indented });
        }

    }
}