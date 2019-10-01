namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Admin.Products;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.DataTransferObjects.Products.MyProducts;
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
        IQueryable<ProductForManagingOutDto> GetAllForManaging();
        IQueryable<ProductMinifiedOutDto> GetMostCommented(int count);
        IQueryable<ProductMinifiedOutDto> GetMostRated(int count);
        IQueryable<ProductMinifiedOutDto> GetAll();
        Task<ProductDetailedOutDto> GetDetails(int id, string UserId = null);
        Task RateByUser(int productId, Grade rating, AppUser user);
        IQueryable<ProductMinifiedOutDto> GetProductsByName(string phrase);
        ICollection<MyCommentedProductDto> GetCommentedProducts(string userId);
        ICollection<MyRatedProductDto> GetRatedProducts(string userId);
        Task ProductFavouriteStatusChange(int productId, string userId);
        ICollection<MyFavouriteProductDto> GetFavouriteProducts(string id);
        Task MarkProductAsDeleted(int productId);
        Task MarkProductAsNotDeleted(int productId);
        Task SetNewQuantity(int productId, uint quantity);
    }
}