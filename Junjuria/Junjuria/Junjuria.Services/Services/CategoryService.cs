namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Admin.Categories;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryService : ICategoryService
    {
        private static object LockObject = new object();

        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public Category GetById(int catId)
        {
            return categoryRepository.All().FirstOrDefault(x => x.Id == catId);
        }

        public ICollection<int> GetSubcategoriesOfCagetoryId(int id)
        {
            HashSet<int> ids = new HashSet<int>();
            var idPool = categoryRepository.All().Where(x => x.CategoryId != null).Select(x => new
            {
                x.Id,
                x.CategoryId
            }).ToArray();

            if (categoryRepository.All().FirstOrDefault(x => x.Id == id) != null) ids.Add(id);
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

        public ICollection<CategoryMiniOutDto> GetAllMinified()
        {
            return categoryRepository.All().To<CategoryMiniOutDto>().ToArray();
        }

        public async Task AddCategoryAsync(CategoryInDto dto)
        {
            var newCategory = mapper.Map<Category>(dto);
            newCategory.CategoryId = newCategory.CategoryId == -1 ? null : newCategory.CategoryId;
            bool isTitleUnique = categoryRepository.All().Select(x => x.Title).All(x => x.ToLower() != dto.Title.ToLower());
            bool isTargetCategoryExisting = (newCategory.CategoryId is null) || categoryRepository.All().Select(x => x.CategoryId).Any(x => newCategory.CategoryId == x);
            if (!isTitleUnique || !isTargetCategoryExisting) return;
            await categoryRepository.AddAssync(newCategory);
            await categoryRepository.SaveChangesAsync();
        }

        public ICollection<CategoryManageItemOutDto> GetAllCategoryManageItems()
        {
            return categoryRepository.All().To<CategoryManageItemOutDto>().ToArray();
        }

        public void DeleteCategory(int categoryId)
        {
            lock (LockObject)
            {
                var category = categoryRepository.All().Include(x => x.SubCategories).Include(x => x.Products).FirstOrDefault(x => x.Id == categoryId);
                if (category is null || category.Products.Any() || category.SubCategories.Any()) return;
                categoryRepository.Remove(category);
                categoryRepository.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }

        public async Task<CategoryOutInDto> GetCategoryInfoAsync(int categoryId)
        {
            var category = await categoryRepository.All().FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category is null) return null;

            return mapper.Map<CategoryOutInDto>(category);
        }


        public void EditCategory(CategoryOutInDto dto)
        {
            lock (LockObject)
            {
                if (dto.CategoryId == -1) dto.CategoryId = null;
                var fatherCategoryIsValid = categoryRepository.All().Any(x => x.Id == dto.CategoryId) || dto.CategoryId == null;
                var category = categoryRepository.All().FirstOrDefault(x => x.Id == dto.Id);
                var editedTitleIsUnique = categoryRepository.All().All(x => x.Id != dto.Id && x.Title.ToLower() != dto.Title.ToLower());
                if (category != null && fatherCategoryIsValid && editedTitleIsUnique)
                {
                    category.Title = dto.Title;
                    category.CategoryId = dto.CategoryId;
                    category.Description = dto.Description;
                    categoryRepository.SaveChangesAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}