namespace Junjuria.Common
{
    using Abp.Net.Mail;
    using AutoMapper;
    using Junjuria.App.ViewComponents.DTO;
    using Junjuria.Common.Contracts;
    using Junjuria.Common.Dtos;
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
    using Junjuria.Services.Services;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class DIContainer : IDIContainer
    {
        private static IServiceProvider Container;
        private static string DbName = "memoryDb";
        public static RelocateInfoImg RelocationInfo { get; private set; } = new RelocateInfoImg();
        public static EmailSent EmailSent { get; private set; } = new EmailSent();

        public static string TestUserId { get; }
        public static string TestUserName { get; }
        public static string TestUserMail { get; }
        public DIContainer()
        {
            RelocationInfo = new RelocateInfoImg();
            EmailSent = new EmailSent();
        }

        static DIContainer()
        {
            Container = RegisterServices();
            TestUserId = "testId12";
            TestUserName = "TesterName";
            TestUserMail = "test@test";
            SeedTestUser();
        }

        private static IServiceProvider RegisterServices()
        {
            var container = new ServiceCollection();
            container.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(DbName));

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

            #region Mocked
            var viewRenderServiceMock = new Mock<IViewRenderService>();
            viewRenderServiceMock.Setup(x => x.RenderToStringAsync(It.IsAny<string>(), It.IsAny<object>())).Returns((string a,object b)=>Task<string>.Run(()=>"Result"));
            container.AddSingleton<IViewRenderService>(viewRenderServiceMock.Object);

            var cloudineryMock = new Mock<ICloudineryService>();
            cloudineryMock.Setup(x => x.RelocateImgToCloudinary(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                          .Returns((string name, string imgPath, string info, bool isUrl) =>
                          {
                              RelocationInfo.Name = name;
                              RelocationInfo.ImgPath = imgPath;
                              RelocationInfo.Info = info;
                              RelocationInfo.IsUrl = isUrl;
                              return $"relocation to Our our Repository: {name}|{imgPath}|{info}|{isUrl}";
                          });
            container.AddSingleton<ICloudineryService>(cloudineryMock.Object);

            var emailSendMock = new Mock<IEmailSender>();
            emailSendMock.Setup(x => x.Send(It.IsAny<MailMessage>(), It.IsAny<bool>()))
                         .Callback((Action<MailMessage, bool>)((MailMessage mail, bool normalize) =>
                         {
                             DIContainer.EmailSent.Mail = mail;
                             DIContainer.EmailSent.Normalised = normalize;
                         }));

            container.AddSingleton<IEmailSender>(emailSendMock.Object);
            #endregion'
            return container.BuildServiceProvider();
        }
        public object GetService(Type serviceType) => Container.GetService(serviceType);
        public static T GetService<T>() => Container.GetService<T>();

        private static void SeedTestUser()
        {
            var userRepository = Container.GetService<IRepository<AppUser>>();
            userRepository.AddAssync(new AppUser
            {
                Id = TestUserId,
                UserName = TestUserName,
                Email = TestUserMail
            }).GetAwaiter().GetResult();
            userRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}