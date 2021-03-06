﻿using Junjuria.DataTransferObjects.Admin.Categories;
using Junjuria.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Junjuria.Services.Services.Contracts
{
    public interface ICategoryService
    {
        /// <summary>
        /// Returns the category and its child categories !
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        ICollection<int> GetSubcategoriesOfCagetoryId(int categoryId);
        Category GetById(int catId);
        ICollection<DataTransferObjects.Admin.Categories.CategoryMiniOutDto> GetAllMinified();
        Task AddCategoryAsync(CategoryInDto dto);
        ICollection<CategoryManageItemOutDto> GetAllCategoryManageItems();
        void DeleteCategory(int categoryId);
        Task<CategoryOutInDto> GetCategoryInfoAsync(int categoryId);
        void EditCategory(CategoryOutInDto dto);
    }
}