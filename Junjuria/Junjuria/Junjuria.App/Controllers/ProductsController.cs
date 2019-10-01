namespace Junjuria.App.Controllers
{
    using Junjuria.Common;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using X.PagedList;

    public class ProductsController : BaseController
    {
        private readonly IProductService productsService;
        private readonly ICategoryService categoriesService;
        private readonly UserManager<AppUser> userManager;

        public ActionResult Index()
        {
            return RedirectToAction("All");
        }

        public ProductsController(IProductService productsService, ICategoryService categoriesService, UserManager<AppUser> userManager)
        {
            this.productsService = productsService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
        }

        public IActionResult Search([Required, MinLength(2)]string phrase, int? pageNum, string returnPath)
        {
            if (ModelState.IsValid)
            {
                int productsFound = productsService.GetProductsByName(phrase).Count();
                ViewBag.PageNavigation = productsFound > GlobalConstants.MaximumCountOfAllProductsOnSinglePage ? "Search" : null;
                ViewData["Phrase"] = phrase;
                var dtos = productsService.GetProductsByName(phrase).ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfAllProductsOnSinglePage);
                ViewData["SubHead1"] = new string[] { "Products matching search phrase", $"\"{phrase}\"" };
                ViewData["SubHead2"] = new string[] { $"{productsFound} matches found", "" };
                return this.View("DisplayProducts", dtos);
            }
            return Redirect(returnPath);
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
            int allProductsCount = productsService.GetAll().Count();
            ViewBag.PageNavigation = allProductsCount > GlobalConstants.MaximumCountOfAllProductsOnSinglePage ? "All" : null;
            var dtos = productsService.GetAll().ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfAllProductsOnSinglePage);
            ViewData["SubHead1"] = new string[] { "All of our products:", $"{allProductsCount} total"};
            return this.View("DisplayProducts", dtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await userManager.GetUserAsync(User);
            var product = await productsService.GetDetails(id, user?.Id);
            return this.View(product);
            // return Json(productsService.GetDetails(id),new Newtonsoft.Json.JsonSerializerSettings { Formatting = Newtonsoft.Json.Formatting.Indented });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RateProduct(int ProductId, Grade Rating)
        {
            var user = await userManager.GetUserAsync(this.User);
            await productsService.RateByUser(ProductId, Rating, user);
            return RedirectToAction("Details", new { id = ProductId });
        }

        [Authorize]
        public async Task<IActionResult> MyCommented()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var dtos = productsService.GetCommentedProducts(currentUser.Id);
            return View(dtos);
        }

        [Authorize]
        public async Task<IActionResult> MyRated()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var dtos = productsService.GetRatedProducts(currentUser.Id);
            return View(dtos);
        }

        [Authorize]
        public async Task<IActionResult> MyFavourite()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var dtos = productsService.GetFavouriteProducts(currentUser.Id);
            return View(dtos);
        }

        [Authorize]
        public async Task FavourizeProduct(int productId)
        {
            var currentUser = await userManager.GetUserAsync(User);
            await productsService.ProductFavouriteStatusChange(productId, currentUser.Id);
        }
    }
}