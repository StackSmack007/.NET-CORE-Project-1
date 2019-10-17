namespace Junjuria.Services.Services
{
    using Junjuria.Common;
    using Junjuria.DataTransferObjects.Admin.Categories;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class CategoryServiceTests
    {
        private readonly ICategoryService categoryService;
        
        private readonly IRepository<Category> categoriesRepository;
        public CategoryServiceTests()
        {
            categoryService = DIContainer.GetService<ICategoryService>();
            categoriesRepository = DIContainer.GetService<IRepository<Category>>();
            SeedData().GetAwaiter().GetResult();
        }

        [Theory]
        [InlineData(1, 2, 3, 4)]
        [InlineData(2, 3)]
        [InlineData(3, null)]
        [InlineData(5, null)]
        public void GetSubcategoriesOfCagetoryId_Returns_SubCategories(int id, params int[] result)
        {
            var expectedResult = new HashSet<int>();
            expectedResult.Add(id);
            if (result != null) expectedResult.UnionWith(result);
            int[] actuallResult = categoryService.GetSubcategoriesOfCagetoryId(id).OrderBy(x => x).ToArray();
            Assert.True(expectedResult.SequenceEqual(actuallResult));
        }

        [Fact]
        public void GetSubcategoriesOfCagetoryId_Returns_EmptyCollection_When_Invalid_Id_Provided()
        {
            int invalidId = -2;
            var actuallResult = categoryService.GetSubcategoriesOfCagetoryId(invalidId).OrderBy(x => x);
            Assert.False(actuallResult.Any());
        }

        [Theory]
        [InlineData(1, "Primary1")]
        [InlineData(2, "SecondTier1")]
        [InlineData(3, "ThirdTear")]
        public void GetById_Returns_Category_When_CorrectId_Provided(int id, string expectedTitle)
        {
            Category result = categoryService.GetById(id);
            Assert.NotNull(result);
            Assert.Equal(expectedTitle, result.Title);
        }

        [Fact]
        public void GetById_returnsNullIfIncorrectIdGiven()
        {
            int invalidId = -2;
            Category result = categoryService.GetById(invalidId);
            Assert.Null(result);
        }

        [Fact]
        public void AddCategory_Adds_NewCategory_When_UniqueTitle_Provided()
        {
            int categoryId = 4;
            string title = "NewAddedCategory";
            string description = "NewCagetoryDescription";

            CategoryInDto categoryNew = new CategoryInDto
            {
                CategoryId = categoryId,
                Title = title,
                Description = description
            };

            int expectedCountAfterAdding = categoriesRepository.All().Count() + 1;
            categoryService.AddCategoryAsync(categoryNew);
            int actualCountAfterAdding = categoriesRepository.All().Count();
            Assert.Equal(expectedCountAfterAdding, actualCountAfterAdding);
            var categoryAdded = categoriesRepository.All().Last();
            Assert.Equal(categoryId, categoryAdded.CategoryId);
            Assert.Equal(title, categoryAdded.Title);
            Assert.Equal(description, categoryAdded.Description);
        }
        [Fact]
        public void AddCategory_DoNot_Add_NewCategory_When_ExistingTitle_Provided()
        {
            int categoryId = 4;
            string repeatingTitle = "Primary1";
            string description = "NewCagetoryDescription";

            CategoryInDto categoryNew = new CategoryInDto
            {
                CategoryId = categoryId,
                Title = repeatingTitle,
                Description = description
            };

            int expectedCountAfterAdding = categoriesRepository.All().Count();
            categoryService.AddCategoryAsync(categoryNew);
            int actualCountAfterAdding = categoriesRepository.All().Count();
            Assert.Equal(expectedCountAfterAdding, actualCountAfterAdding);
            var categoryAdded = categoriesRepository.All().Last();
            Assert.NotEqual(categoryId, categoryAdded.CategoryId);
            Assert.NotEqual(repeatingTitle, categoryAdded.Title);
            Assert.NotEqual(description, categoryAdded.Description);
        }

        [Fact]
        public void AddCategory_DoNot_Add_NewCategory_When_NonExistingTargetId_Provided()
        {
            int categoryId = 999;
            string repeatingTitle = "NewCategory2";
            string description = "NewCagetoryDescription";

            CategoryInDto categoryNew = new CategoryInDto
            {
                CategoryId = categoryId,
                Title = repeatingTitle,
                Description = description
            };

            int expectedCountAfterAdding = categoriesRepository.All().Count();
            categoryService.AddCategoryAsync(categoryNew);
            int actualCountAfterAdding = categoriesRepository.All().Count();
            Assert.Equal(expectedCountAfterAdding, actualCountAfterAdding);
            var categoryAdded = categoriesRepository.All().Last();
            Assert.NotEqual(categoryId, categoryAdded.CategoryId);
            Assert.NotEqual(repeatingTitle, categoryAdded.Title);
            Assert.NotEqual(description, categoryAdded.Description);
        }

        [Theory]
        [InlineData(1, "Primary1", null)]
        [InlineData(2, "SecondTier1", 1)]
        [InlineData(3, "ThirdTear", 2)]
        [InlineData(4, "SecondTier2", 1)]
        [InlineData(5, "Primary2", null)]
        public async Task GetCategoryInfo_Returns_CategoryInfo_When_CorrectId_Provided(int id, string title, int? categoryId)
        {
            CategoryOutInDto categoryInfo = await categoryService.GetCategoryInfoAsync(id);
            Assert.NotNull(categoryInfo);
            Assert.Equal(id, categoryInfo.Id);
            Assert.Equal(title, categoryInfo.Title);
            Assert.Equal(categoryId, categoryInfo.CategoryId);
        }

        [Fact]
        public async Task GetCategoryInfo_Returns_Null_When_IncorrectId_Provided()
        {
            int incorrectId = -20;
            var categoryInfo = await categoryService.GetCategoryInfoAsync(incorrectId);
            Assert.Null(categoryInfo);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(4)]
        public async Task DeleteCategory_Removes_Category_When_Empty_ExistingCategory_Id_Provided(int emptyExistingCategoryId)
        {

            int expectedCategoriesCount = categoriesRepository.All().Count() - 1;
            categoryService.DeleteCategory(emptyExistingCategoryId);
            int actualCategoriesCount = categoriesRepository.All().Count();
            var categoryFound = categoriesRepository.All().FirstOrDefault(x => x.Id == emptyExistingCategoryId);
            Assert.Null(categoryFound);
            Assert.Equal(expectedCategoriesCount, actualCategoriesCount);

            var categoriesLeft = categoriesRepository.All().ToArray();
            categoriesRepository.RemoveRange(categoriesLeft);
            await categoriesRepository.SaveChangesAsync();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void DeleteCategory_DoNot_Remove_Category_Correctly_When_NonEmpty_Existing_Id_Provided(int nonEmptyExistingId)
        {
            int expectedCategoriesCount = categoriesRepository.All().Count();
            categoryService.DeleteCategory(nonEmptyExistingId);
            int actualCategoriesCount = categoriesRepository.All().Count();
            var categoryFound = categoriesRepository.All().FirstOrDefault(x => x.Id == nonEmptyExistingId);
            Assert.NotNull(categoryFound);
            Assert.Equal(expectedCategoriesCount, actualCategoriesCount);

        }

        [Fact]
        public void DeleteCategory_DoNot_Remove_Category_When_NonExisting_Id_Provided()
        {
            int nonExistingId = -10;
            int expectedCategoriesCount = categoriesRepository.All().Count();
            categoryService.DeleteCategory(nonExistingId);
            int actualCategoriesCount = categoriesRepository.All().Count();
            var categoryFound = categoriesRepository.All().FirstOrDefault(x => x.Id == nonExistingId);
            Assert.Null(categoryFound);
            Assert.Equal(expectedCategoriesCount, actualCategoriesCount);
        }

        [Fact]
        public async Task EditCategory_Edits_Category_When_Correct_Id_Provided()
        {
            int targetId = 1;
            int categoriesCountBefore = categoriesRepository.All().Count();

            var editedCategory = new CategoryOutInDto
            {
                Id = targetId,
                Title = "EditedTitle",
                CategoryId = 5,
                Description = "EditedDescription"
            };
            categoryService.EditCategory(editedCategory);
            int categoriesCountAfter = categoriesRepository.All().Count();
            Assert.Equal(categoriesCountAfter, categoriesCountBefore);
            var categoryFound = await categoriesRepository.All().FirstOrDefaultAsync(x => x.Id == targetId);
            Assert.NotNull(categoryFound);
            Assert.Equal(editedCategory.Title, categoryFound.Title);
            Assert.Equal(editedCategory.CategoryId, categoryFound.CategoryId);
            Assert.Equal(editedCategory.Description, categoryFound.Description);

            var categoriesLeft = categoriesRepository.All().ToArray();
            categoriesRepository.RemoveRange(categoriesLeft);
            await categoriesRepository.SaveChangesAsync();
        }

        [Fact]
        public async Task EditCategory_DoNotEditCategory_When_NotCorrect_Id_Provided()
        {
            int targetId = -10;
            int categoriesCountBefore = categoriesRepository.All().Count();
            var editedCategory = new CategoryOutInDto
            {
                Id = targetId,
                Title = "EditedTitle",
                CategoryId = 5,
                Description = "EditedDescription"
            };
            categoryService.EditCategory(editedCategory);
            int categoriesCountAfter = categoriesRepository.All().Count();
            Assert.Equal(categoriesCountAfter, categoriesCountBefore);
            var categoryFound = await categoriesRepository.All().FirstOrDefaultAsync(x => x.Id == targetId);
            Assert.Null(categoryFound);
            Assert.False(categoriesRepository.All().Any(x => x.Title == editedCategory.Title || x.Description == editedCategory.Description));
        }

        [Fact]
        public async Task EditCategory_DoNotEditCategory_When_Incorrect_TargetId_Provided()
        {
            int targetId = 1;
            int invalidTargetId = -10;
            int categoriesCountBefore = categoriesRepository.All().Count();
            var editedCategory = new CategoryOutInDto
            {
                Id = targetId,
                Title = "EditedTitle",
                CategoryId = invalidTargetId,
                Description = "EditedDescription"
            };
            categoryService.EditCategory(editedCategory);
            int categoriesCountAfter = categoriesRepository.All().Count();
            Assert.Equal(categoriesCountAfter, categoriesCountBefore);
            var categoryFound = await categoriesRepository.All().FirstOrDefaultAsync(x => x.Id == targetId);
            Assert.NotNull(categoryFound);
            Assert.NotEqual(categoryFound.Title, editedCategory.Title);
            Assert.NotEqual(categoryFound.Description, editedCategory.Description);
        }

        [Fact]
        public async Task EditCategory_DoNotEditCategory_When_ExistingTitle_Provided()
        {
            int targetId = 1;
            string existingTitle = "Primary2";
            int categoriesCountBefore = categoriesRepository.All().Count();
            var editedCategory = new CategoryOutInDto
            {
                Id = targetId,
                Title = existingTitle,
                CategoryId = null,
                Description = "EditedDescription"
            };
            categoryService.EditCategory(editedCategory);
            int categoriesCountAfter = categoriesRepository.All().Count();
            Assert.Equal(categoriesCountAfter, categoriesCountBefore);
            var categoryFound = await categoriesRepository.All().FirstOrDefaultAsync(x => x.Id == targetId);
            Assert.NotNull(categoryFound);
            Assert.NotEqual(categoryFound.Title, editedCategory.Title);
            Assert.NotEqual(categoryFound.Description, editedCategory.Description);
        }

        private async Task SeedData()
        {
            if (!categoriesRepository.All().Any())
            {
                var categoriesData = new List<Category>
            {
                new Category
                {
                    Id=1,
                    Title="Primary1",
                    CategoryId=null
                },
                new Category
                {
                    Id=2,
                    Title="SecondTier1",
                    CategoryId=1
                },
                new Category
                {
                    Id=3,
                    Title="ThirdTear",
                    CategoryId=2
                },
                new Category
                {
                    Id=4,
                    Title="SecondTier2",
                    CategoryId=1
                },
                new Category
                {
                    Id=5,
                    Title="Primary2",
                    CategoryId=null
                },
            };
                await categoriesRepository.AddRangeAssync(categoriesData);
                await categoriesRepository.SaveChangesAsync();
            }
        }
    }
}