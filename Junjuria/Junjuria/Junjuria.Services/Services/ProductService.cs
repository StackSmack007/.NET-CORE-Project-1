namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Admin.Products;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.DataTransferObjects.Products.MyProducts;
    using Junjuria.DataTransferObjects.Products.MyProducts.Favouring;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProductService : IProductService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Product> productsRepository;

        private readonly IRepository<ProductComment> commentRepository;

        private readonly IRepository<UserFavouriteProduct> userFavProdRepository;

        public ProductService(IMapper mapper,
                              IRepository<Product> productsRepository,
                              IRepository<ProductComment> commentRepository,
                              IRepository<UserFavouriteProduct> userFavProdRepository)
        {
            this.mapper = mapper;
            this.productsRepository = productsRepository;

            this.commentRepository = commentRepository;

            this.userFavProdRepository = userFavProdRepository;
        }

        public IQueryable<ProductMinifiedOutDto> GetProductsByCategories(ICollection<int> categoriesIds)
        {
            var dtos = productsRepository.All().Where(x => categoriesIds.ToArray().Contains(x.CategoryId) && !x.IsDeleted)
                                              .To<ProductMinifiedOutDto>();
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetOnSale()
        {
            var dtos = productsRepository.All().Where(x => x.Discount > 0 && x.Quantity != 0 && !x.IsDeleted)
                                               .OrderByDescending(x => x.Discount)
                                               .To<ProductMinifiedOutDto>();
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetMostPurchased(int count)
        {
            var dtos = productsRepository.All().Where(x => !x.IsDeleted)
                                               .OrderByDescending(x => x.ProductOrders.Count)
                                               .To<ProductMinifiedOutDto>().Take(count);
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetMostCommented(int count)
        {
            var dtos = productsRepository.All().Where(x => !x.IsDeleted)
                                               .OrderByDescending(x => x.ProductComments.Count)
                                               .To<ProductMinifiedOutDto>().Take(count);
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetMostRated(int count)
        {
            var dtos = productsRepository.All().Where(x => !x.IsDeleted)
                                               .OrderByDescending(x => x.Votes.Count)
                                               .To<ProductMinifiedOutDto>()
                                               .OrderByDescending(x => x.Grade).Take(count);
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetAll()
        {
            var dtos = productsRepository.All().Where(x => !x.IsDeleted)
                                               .To<ProductMinifiedOutDto>()
                                               .OrderByDescending(x => x.IsAvailable)
                                               .ThenBy(x => x.Price);
            return dtos;
        }

        public async Task<ProductDetailedOutDto> GetDetails(int id, string UserId = null)
        {
            var product = productsRepository.All().To<ProductDetailedOutDto>().FirstOrDefault(x => x.Id == id);
            if (product is null) return null;
            product.IsFavourite = await userFavProdRepository.All().AnyAsync(x => x.UserId == UserId && x.ProductId == id);
            return product;
        }

        public async Task RateByUserAsync(int productId, Grade rating, AppUser user)
        {
            Product product = await productsRepository.All().Where(x => x.Id == productId).Include(x => x.Votes).FirstOrDefaultAsync();
            var voteOfThisUser = product.Votes.FirstOrDefault(x => x.UserId == user.Id);
            if (voteOfThisUser is null)
            {
                product.Votes.Add(new ProductVote
                {
                    Voter = user,
                    Grade = rating
                });
            }
            else
            {
                voteOfThisUser.Grade = rating;
            }
            await productsRepository.SaveChangesAsync();
        }

        public IQueryable<ProductMinifiedOutDto> GetProductsByName(string phrase)
        {
            var dtos = productsRepository.All().Where(x => !x.IsDeleted).To<ProductMinifiedOutDto>()
                     .Where(x => x.Name.ToLower().Contains(phrase.ToLower()))
                                   .OrderByDescending(x => x.IsAvailable)
                                   .ThenBy(x => x.Price);
            return dtos;
        }

        public ICollection<MyCommentedProductDto> GetCommentedProducts(string userId)
        {
            var result = commentRepository.All().Where(x => x.AuthorId == userId)
     .GroupBy(x => x.ProductId, (key, c) => new { ProductId = key, Comment = c.OrderByDescending(cm => cm.DateOfCreation).First() })
     .Select(x => new MyCommentedProductDto
     {
         Id = x.ProductId,
         Name = x.Comment.Product.Name,
         DiscountedPrice = x.Comment.Product.DiscountedPrice,
         LastComment = x.Comment.Comment,
         LastCommentedDate = x.Comment.DateOfCreation
     }).ToArray();

            var productComments = productsRepository.All().Select(x => new
            {
                x.Id,
                ComentsCount = x.ProductComments.Count(),
                MyCommentCount = x.ProductComments.Count(c => c.AuthorId == userId),
            }).Where(x => x.MyCommentCount > 0).ToArray();
            foreach (var item in productComments)
            {
                var target = result.FirstOrDefault(x => x.Id == item.Id);
                target.ComentsCount = item.ComentsCount;
                target.MyCommentCount = item.MyCommentCount;
            }
            return result;
        }

        public ICollection<MyRatedProductDto> GetRatedProducts(string userId)
        {
            var result = productsRepository.All().Where(x => x.Votes.Any(v => v.UserId == userId))
                .Include(x => x.Votes)
                .Select(x => new MyRatedProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    GradeTotal = (Grade)(int)Math.Round((double)x.Votes.Sum(v => (int)v.Grade) / x.Votes.Count()),
                    MyGrade = x.Votes.Where(v => v.UserId == userId).FirstOrDefault().Grade,
                    DiscountedPrice = x.DiscountedPrice
                }).ToArray();
            return result;
        }

        public ICollection<MyFavouriteProductDto> GetFavouriteProducts(string userId)
        {
            var result = productsRepository.All().Where(x => x.UsersFavouringThisProduct.Any(u => u.UserId == userId)).To<MyFavouriteProductDto>().ToArray();
            return result;
        }


        public IQueryable<ProductForManagingOutDto> GetAllForManaging()
        {
            var products = productsRepository.All().OrderBy(x => x.IsDeleted).ThenBy(x => x.Category.Title).To<ProductForManagingOutDto>();
            return products;
        }

        public async Task MarkProductAsDeletedAsync(int productId)
        {
            var product = await productsRepository.All().FirstOrDefaultAsync(x => x.Id == productId);
            if (product != null && !product.IsDeleted)
            {
                product.IsDeleted = true;
                await productsRepository.SaveChangesAsync();
            }
        }

        public async Task MarkProductAsNotDeletedAsync(int productId)
        {
            var product = await productsRepository.All().FirstOrDefaultAsync(x => x.Id == productId);
            if (product != null && product.IsDeleted)
            {
                product.IsDeleted = false;
                await productsRepository.SaveChangesAsync();
            }
        }

        public async Task SetNewQuantityAsync(int productId, uint quantity)
        {
            var product = await productsRepository.All().FirstOrDefaultAsync(x => x.Id == productId);
            if (product is null) return;
            lock (ConcurencyMaster.LockProductsObj)
            {
                product.Quantity = quantity;
                productsRepository.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }

        public async Task AddNewProductAsync(NewProductInDto dto)
        {
            var newProduct = mapper.Map<Product>(dto);
            foreach (var characteristic in dto.Characteristics.Distinct())
            {
                newProduct.Characteristics.Add(new ProductCharacteristic
                {
                    Title = characteristic.Title,
                    TextValue = characteristic.TextValue,
                    NumericValue = characteristic.NumericValue
                });
            }

            foreach (var pic in dto.ProductPictures.Distinct())
            {
                newProduct.ProductPictures.Add(new ProductPicture
                {
                    PictureURL = pic.PictureURL,
                    PictureDescription = pic.PictureDescription,
                });
            }
            await productsRepository.AddAssync(newProduct);
            await productsRepository.SaveChangesAsync();
        }

        public async Task<EditProductOutDto> GetEditableProductAsync(int productId)
        {
            var product = await productsRepository.All()
                                                  .To<EditProductOutDto>()
                                                  .FirstOrDefaultAsync(x => x.Id == productId);
            if (product is null) return null;
            return product;
        }

        public async Task ModifyProductAsync(EditProductOutDto dto)
        {

            Product product = await productsRepository.All()
                                                      .Include(x => x.Votes)
                                                      .Include(x => x.Characteristics)
                                                      .Include(x => x.ProductPictures)
                                                      .Include(x => x.ProductComments)
                                                      .FirstOrDefaultAsync(x => x.Id == dto.Id);

            product.Name = dto.Name;
            product.CategoryId = dto.CategoryId;
            product.ManufacturerId = dto.ManufacturerId;
            product.Price = dto.Price;
            product.Discount = dto.Discount;
            product.Quantity = dto.Quantity;
            product.Weight = dto.Weight;
            product.MonthsWarranty = dto.MonthsWarranty;
            product.MainPicURL = dto.MainPicURL;
            product.ReviewURL = dto.ReviewURL;
            product.Description = dto.Description;
            product.IsDeleted = dto.IsDeleted;

            if (!dto.VotesAny && product.Votes.Any())
            {
                product.Votes = new HashSet<ProductVote>();
            }

            product.Characteristics = new HashSet<ProductCharacteristic>();
            foreach (NewProductCharacteristicDto dtoChar in dto.Characteristics)
            {
                product.Characteristics.Add(mapper.Map<ProductCharacteristic>(dtoChar));
            }

            product.ProductPictures = new HashSet<ProductPicture>();
            foreach (var dtoPicture in dto.ProductPictures)
            {
                product.ProductPictures.Add(mapper.Map<ProductPicture>(dtoPicture));
            }

            foreach (var dtoComment in dto.ProductComments)
            {
                var productComment = product.ProductComments.FirstOrDefault(x => x.Id == dtoComment.Id);
                if (productComment is null) continue;
                productComment.IsDeleted = dtoComment.IsDeleted;
                productComment.Comment = dtoComment.Comment;
            }

            lock (ConcurencyMaster.LockProductsObj)
            {
                productsRepository.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }

        public async Task<FavouringResponseOutDto> FavourizeAsync(ChoiseOfFavouringProductDto dto, string userId)
        {
            var result = new FavouringResponseOutDto {IsPositive=dto.Choise };
           
            int[] usersFavouriteProducts = await userFavProdRepository.All().Where(x => x.UserId == userId).Select(x => x.ProductId).ToArrayAsync();

            if (!(dto.Choise ^ usersFavouriteProducts.Contains(dto.ProductId))) return null;//fail
            if (dto.Choise)
            {
                await userFavProdRepository.AddAssync(new UserFavouriteProduct
                {
                    UserId = userId,
                    ProductId = dto.ProductId
                });
                result.TotalFavouredProducts = usersFavouriteProducts.Length + 1;
            }
            else
            {
                var favProduct = await userFavProdRepository.All().SingleAsync(x => x.ProductId == dto.ProductId && x.UserId == userId);
                userFavProdRepository.Remove(favProduct);
                result.TotalFavouredProducts = usersFavouriteProducts.Length - 1;
            }
            await userFavProdRepository.SaveChangesAsync();
            return result;
        }
        #region depricated
        ///public async Task ProductFavouriteStatusChangeAsync(int productId, string userId)
        ///{
        ///    var fav = await userFavProdRepository.All().FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        ///    if (fav is null)
        ///    {
        ///        await userFavProdRepository.AddAssync(new UserFavouriteProduct
        ///        {
        ///            ProductId = productId,
        ///            UserId = userId
        ///        });
        ///    }
        ///    else
        ///    {
        ///        userFavProdRepository.Remove(fav);
        ///    }
        ///    await userFavProdRepository.SaveChangesAsync();
        ///}
#endregion
    }
}