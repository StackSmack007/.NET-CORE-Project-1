﻿namespace Junjuria.App.Areas.Admin.Controllers
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
            var categoryDtos = categoryService.GetAllCategoryManageItems();
            return View(categoryDtos);
        }


        public IActionResult Create()
        {
            ViewData["ExistingCategories"] = categoryService.GetAllMinified();
            return View();
        }
        [HttpPost]
        public IActionResult Delete(int categoryId)
        {
            categoryService.DeleteCategory(categoryId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryInDto dto)
        {
            ViewData["ExistingCategories"] = categoryService.GetAllMinified();
            if (((ICollection<CategoryMiniOutDto>)ViewData["ExistingCategories"]).Any(x => x.Title.ToLower() == dto.Title.ToLower()))
            {
                ModelState.AddModelError("NameTaken", $"Name {dto.Title} is already used for category name!");
            }
            if (ModelState.IsValid)
            {
                await categoryService.AddCategory(dto);
                return RedirectToAction(nameof(Manage));
            }
            return View(dto);
        }
        public async Task<IActionResult> Edit(int categoryId)
        {
            ViewData["ExistingCategories"] = categoryService.GetAllMinified();
            CategoryInDto categoryDtos = await categoryService.GetCategoryInfo(categoryId);
            return View(categoryDtos);
        }

        [HttpPost]
        public  IActionResult Edit(CategoryOutInDto dto)
        {
            ViewData["ExistingCategories"] = categoryService.GetAllMinified();
            if (((ICollection<CategoryMiniOutDto>)ViewData["ExistingCategories"]).Any(x => x.Title.ToLower() == dto.Title.ToLower() && x.Id != dto.Id))
            {
                ModelState.AddModelError("NameTaken", $"Name {dto.Title} is already used for category name!");
            }
            if (dto.CategoryId == dto.Id)
            {
                ModelState.AddModelError("Circular Reference", $"Category can not be in itself");
            }
            if (ModelState.IsValid)
            {
                categoryService.EditCategory(dto);
                return RedirectToAction(nameof(Manage));
            }
            return View(dto);
        }

    }
}