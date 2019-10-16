namespace Junjuria.Services.Services
{
    using Abp.Net.Mail;
    using AutoMapper;
    using Junjuria.App.ViewComponents.DTO;
    using Junjuria.DataTransferObjects.Admin.Categories;
    using Junjuria.DataTransferObjects.Admin.Manufacturers;
    using Junjuria.DataTransferObjects.Admin.Products;
    using Junjuria.DataTransferObjects.Manufacturers;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.DataTransferObjects.Products.MyProducts;
    using Junjuria.DataTransferObjects.RecomendationsPage;
    using Junjuria.Infrastructure.Data;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Xunit;

    public class CategoryServiceTests
    {
        private readonly ICategoryService categoryService;
        IServiceProvider DIContainer;
        private RelocateInfoImg relocationInfo;
        private EmailSent emailSent;
        IRepository<Category> categoriesRepository;
        public CategoryServiceTests()
        {
            DIContainer = RegisterServices();
            relocationInfo = new RelocateInfoImg();
            emailSent = new EmailSent();
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
        public void AddCategory_AddsNewCategory()
        {
            int categoryId = 42;
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
            Assert.False(categoriesRepository.All().Any(x=>x.Title==editedCategory.Title||x.Description==editedCategory.Description));
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
            Assert.NotEqual(categoryFound.Title , editedCategory.Title);
            Assert.NotEqual(categoryFound.Description, editedCategory.Description);
        }

        private IServiceProvider RegisterServices()
        {
            var container = new ServiceCollection();
            container.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("memoryDb"));

            var mappingConfig = new MapperConfiguration(conf =>
            {
                #region RegisterMappingsManually
                conf.CreateMap<Product, ProductDetailedOutDto>()
                    .ForMember(d => d.ProductPictures, opt => opt.MapFrom(s => s.ProductPictures.Select(pctr => pctr.PictureURL).ToArray()));
                conf.CreateMap<Product, PurchaseItemDto>()
                    .ForMember(d => d.Quantity, opt => opt.Ignore());
                conf.CreateMap<Product, ProductForManagingOutDto>()
                    .ForMember(d => d.ProductOrdersPending, opt => opt.MapFrom(s => s.ProductOrders.Count(po => po.Order.Status != Status.Finalised)))
                    .ForMember(d => d.OrderedQuantityPending, opt => opt.MapFrom(s => s.ProductOrders.Where(o => o.Order.Status != Status.Finalised).Sum(po => po.Quantity)))
                    .ForMember(d => d.ProductOrdersTotal, opt => opt.MapFrom(s => s.ProductOrders.Count()))
                    .ForMember(d => d.OrderedQuantityTotal, opt => opt.MapFrom(s => s.ProductOrders.Sum(po => po.Quantity)));
                conf.CreateMap<Product, ProductMinifiedOutDto>()
                    .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s => s.Quantity > 0))
                    .ForMember(d => d.ComentsCount, opt => opt.MapFrom(s => s.ProductComments.Count))
                    .ForMember(d => d.Grade, opt => opt.MapFrom(s =>
                               s.Votes.Any() ? (Grade)(int)Math.Round((double)s.Votes.Sum(x => (int)x.Grade) / s.Votes.Count()) : Grade.NotRated))
                    .ForMember(d => d.OrdersCount, opt => opt.MapFrom(s => s.ProductOrders.Count));
                conf.CreateMap<Order, OrderOutMinifiedDto>()
                    .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.OrderProducts.Select(x => (x.Quantity) * (x.Product.Price)).Sum()))
                    .ForMember(d => d.TotalWeight, opt => opt.MapFrom(s => s.OrderProducts.Select(x => (x.Quantity) * (x.Product.Weight)).Sum()));

                conf.CreateMap<NewProductInDto, Product>()
                    .ForMember(d => d.ProductPictures, opt => opt.Ignore())
                    .ForMember(d => d.Characteristics, opt => opt.Ignore());

                conf.CreateMap<Order, OrderForManaging>()
                    .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.OrderProducts.Select(x => (x.Quantity) * (x.Product.Price)).Sum()))
                    .ForMember(d => d.TotalWeight, opt => opt.MapFrom(s => s.OrderProducts.Select(x => (x.Quantity) * (x.Product.Weight)).Sum()));
                conf.CreateMap<RecomendationInDto, Recomendation>();
                conf.CreateMap<CommentCreateInDto, ProductComment>();
                conf.CreateMap<NewProductCharacteristicDto, ProductCharacteristic>();
                conf.CreateMap<NewProductPictureDto, ProductPicture>();
                conf.CreateMap<ManufacturerEditDto, Manufacturer>();
                conf.CreateMap<ManufacturerInDto, Manufacturer>();
                conf.CreateMap<CategoryInDto, Category>();
                conf.CreateMap<CategoryOutInDto, Category>();
                conf.CreateMap<Manufacturer, ManufacturerOutDto>();
                conf.CreateMap<Product, ProductQuantityDto>();
                conf.CreateMap<Recomendation, RecomendationOutDto>();
                conf.CreateMap<Product, OrderBaseProduct>();
                conf.CreateMap<Order, OrderDetailsOutDto>();
                conf.CreateMap<Order, OrderOutMinifiedDto>();
                conf.CreateMap<ProductOrder, ProductInOrderDto>();
                conf.CreateMap<Product, ProductQuantityDto>();
                conf.CreateMap<Product, ProductWarranty>();
                conf.CreateMap<PurchaseItemDto, PurchaseItemDetailedDto>();
                conf.CreateMap<Manufacturer, ManufacturerDetailsOutDto>();
                conf.CreateMap<Product, ManufacturerProductMiniOutDto>();
                conf.CreateMap<ProductCharacteristic, ProductCharacteristicOutDto>();
                conf.CreateMap<ProductComment, ProductCommentOutDto>();
                conf.CreateMap<CommentSympathy, ProductCommentSympathyOutDto>();
                conf.CreateMap<ProductVote, ProductVoteDto>();
                conf.CreateMap<Product, MyFavouriteProductDto>();
                conf.CreateMap<ProductComment, EditProductCommentDto>();
                conf.CreateMap<Product, EditProductOutDto>();
                conf.CreateMap<ProductCharacteristic, NewProductCharacteristicDto>();
                conf.CreateMap<ProductPicture, NewProductPictureDto>();
                conf.CreateMap<Manufacturer, ManufacturerEditDto>();
                conf.CreateMap<Manufacturer, ManufacturerManageInfoOutData>();
                conf.CreateMap<Manufacturer, ManufacturerMiniOutDto>();
                conf.CreateMap<Category, CategoryManageItemOutDto>();
                conf.CreateMap<Category, CategoryMiniOutDto>();
                conf.CreateMap<Category, CategoryOutInDto>();
                conf.CreateMap<Product, CategoryProductOutDto>();
                #endregion
            });

            IMapper mapper = mappingConfig.CreateMapper();
            container.AddSingleton(mapper);
            container.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
            container.AddScoped<IProductService, ProductService>();
            container.AddScoped<ICategoryService, CategoryService>();
            container.AddScoped<ICommentService, CommentService>();
            container.AddScoped<IOrderService, OrderService>();
            container.AddScoped<IStatisticService, StatisticService>();
            container.AddScoped<IManufacturersService, ManufacturersService>();
            container.AddScoped<IViewRenderService, ViewRenderService>();

            #region Mocked
            var cloudineryMock = new Mock<ICloudineryService>();
            cloudineryMock.Setup(x => x.RelocateImgToCloudinary(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                          .Returns((string name, string imgPath, string info, bool isUrl) =>
                          {
                              relocationInfo.Name = name;
                              relocationInfo.ImgPath = imgPath;
                              relocationInfo.Info = info;
                              relocationInfo.IsUrl = isUrl;
                              return $"relocation to Our our Repository: {name}|{imgPath}|{info}|{isUrl}";
                          });
            container.AddSingleton(typeof(ICloudineryService), cloudineryMock.Object.GetType());

            var emailSendMock = new Mock<IEmailSender>();
            emailSendMock.Setup(x => x.Send(It.IsAny<MailMessage>(), It.IsAny<bool>()))
                         .Callback((MailMessage mail, bool normalize) =>
                         {
                             emailSent.Mail = mail;
                             emailSent.Normalised = normalize;
                         });

            container.AddScoped(typeof(IEmailSender), emailSendMock.Object.GetType());
            #endregion'
            return container.BuildServiceProvider();
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

        private class RelocateInfoImg
        {
            public string Name { get; set; }
            public string ImgPath { get; set; }
            public string Info { get; set; }
            public bool IsUrl { get; set; }
        }

        private class EmailSent
        {
            public MailMessage Mail { get; set; }
            public bool Normalised { get; set; }
        }
    }
}