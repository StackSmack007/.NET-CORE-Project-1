namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProductService : IProductService
    {
        private readonly IRepository<Product> productsRepository;

        private readonly IMapper mapper;
        private readonly IRepository<ProductComment> commentRepository;

        public ProductService(IRepository<Product> productsRepository, IMapper mapper, IRepository<ProductComment> commentRepository)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper;
            this.commentRepository = commentRepository;
        }

        public IQueryable<ProductMinifiedOutDto> GetProductsByCategories(ICollection<int> categoriesIds)
        {
            var dtos = productsRepository.All().Where(x => categoriesIds.Contains(x.CategoryId))
                                              .To<ProductMinifiedOutDto>();
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetOnSale()
        {
            var dtos = productsRepository.All().Where(x => x.Discount > 0 && x.Quantity != 0).OrderByDescending(x => x.Discount)
                                             .To<ProductMinifiedOutDto>();
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetMostPurchased(int count)
        {
            var dtos = productsRepository.All().OrderByDescending(x => x.ProductOrders.Count)
                                              .To<ProductMinifiedOutDto>().Take(count);
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetMostCommented(int count)
        {
            var dtos = productsRepository.All().OrderByDescending(x => x.ProductComments.Count)
                                              .To<ProductMinifiedOutDto>().Take(count);
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetMostRated(int count)
        {
            var dtos = productsRepository.All().OrderByDescending(x => x.Votes.Count).To<ProductMinifiedOutDto>()
                .OrderByDescending(x => x.Grade).Take(count);
            return dtos;
        }

        public IQueryable<ProductMinifiedOutDto> GetAll()
        {
            var dtos = productsRepository.All().To<ProductMinifiedOutDto>()
                                              .OrderByDescending(x => x.IsAvailable)
                                              .ThenBy(x => x.Price);
            return dtos;
        }

        public ProductDetailedOutDto GetDetails(int id)
        {
            var product = productsRepository.All().To<ProductDetailedOutDto>().FirstOrDefault(x => x.Id == id);
            if (product is null) return null;
            return mapper.Map<ProductDetailedOutDto>(product);
        }

        public async Task RateByUser(int productId, Grade rating, AppUser user)
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
            var dtos = productsRepository.All().To<ProductMinifiedOutDto>()
                     .Where(x => x.Name.ToLower().Contains(phrase.ToLower()))
                                   .OrderByDescending(x => x.IsAvailable)
                                   .ThenBy(x => x.Price);
            return dtos;
        }

        public ICollection<MyCommentedProductsDto> GetCommentedProducts(AppUser currentUser)
        {
            //var testR = commentRepository.All().Where(x => x.AuthorId == currentUser.Id).Include(x => x.Product).ThenInclude(x=>x.ProductComments)
            //    .GroupBy(x => x.ProductId/*,x=>x.DateOfCreation*/, (key, c) => new { ProductId = key, Comment = c.OrderByDescending(cm => cm.DateOfCreation).First() })
            //    .Select(x => new MyCommentedProductsDto
            //    {
            //        Id = x.ProductId,
            //        Name = x.Comment.Product.Name,
            //        DiscountedPrice = x.Comment.Product.DiscountedPrice,
            //        ComentsCount = x.Comment.Product.ProductComments.Count(),
            //   //     MyCommentCount = x.Comment.Product.ProductComments.Where(c => c.AuthorId == currentUser.Id).Count(),
            //        LastComment = x.Comment.Comment,
            //        LastCommentedDate = x.Comment.DateOfCreation
            //    });


            var result = commentRepository.All().Where(x => x.AuthorId == currentUser.Id)
     .GroupBy(x => x.ProductId, (key, c) => new { ProductId = key, Comment = c.OrderByDescending(cm => cm.DateOfCreation).First() })
     .Select(x => new MyCommentedProductsDto
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
                MyCommentCount = x.ProductComments.Count(c => c.AuthorId == currentUser.Id),
            }).Where(x => x.MyCommentCount > 0).ToArray();
            foreach (var item in productComments)
            {
                var target = result.FirstOrDefault(x => x.Id == item.Id);
                target.ComentsCount = item.ComentsCount;
                target.MyCommentCount = item.MyCommentCount;
            }
            return result;
        }
    }
}