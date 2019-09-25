namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IProductService
    {
        IQueryable<ProductMinifiedOutDto> GetProductsByCategories(ICollection<int> categoriesIds);
        IQueryable<ProductMinifiedOutDto> GetOnSale();
        IQueryable<ProductMinifiedOutDto> GetMostPurchased(int count);
        IQueryable<ProductMinifiedOutDto> GetMostCommented(int count);
        IQueryable<ProductMinifiedOutDto> GetMostRated(int count);
        IQueryable<ProductMinifiedOutDto> GetAll();
        ProductDetailedOutDto GetDetails(int id);
        Task RateByUser(int productId, Grade rating, AppUser user);
        IQueryable<ProductMinifiedOutDto> GetProductsByName(string phrase);
        ICollection<MyCommentedProductsDto> GetCommentedProducts(AppUser currentUser);
    }
}