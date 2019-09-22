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

        public ProductService(IRepository<Product> productsRepository, IMapper mapper)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper;
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
    }
}