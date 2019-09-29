namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.App.Controllers;
    using Junjuria.DataTransferObjects.Admin.Categories;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class Categories : BaseController
    {
        private readonly ICategoryService categoryService;

        public Categories(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public IActionResult Manage()
        {
            var categoryDtos= categoryService.GetAllCategoryManageItems();
            return View(categoryDtos);
        }

        public IActionResult Add()
        {
            ViewData["ExistingCategories"] = categoryService.GetAllMinified();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int categoryId)
        {
        await  categoryService.DeleteCategory(categoryId);
            return RedirectToAction(nameof(Manage));
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(CategoryInDto dto)
        {
            ViewData["ExistingCategories"] = categoryService.GetAllMinified();
            if (((ICollection<CategoryMiniOutDto>)ViewData["ExistingCategories"]).Any(x => x.Title.ToLower() == dto.Title.ToLower()))
            {
                ModelState.AddModelError("NameTaken", $"Name {dto.Title} is already used for category name!");
            }
            if (ModelState.IsValid)
            {
                await categoryService.AddCategory(dto);
                TempData.Clear();
                return RedirectToAction(nameof(Manage));
            }
            return View(dto);
        }
    }
}