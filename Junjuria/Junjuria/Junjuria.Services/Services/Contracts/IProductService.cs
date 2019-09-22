namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Products;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProductService
    {
        IQueryable<ProductMinifiedOutDto> GetProductsByCategories(ICollection<int> categoriesIds);
        IQueryable<ProductMinifiedOutDto> GetOnSale();
        IQueryable<ProductMinifiedOutDto> GetMostPurchased(int count);
        IQueryable<ProductMinifiedOutDto> GetMostCommented(int count);
        IQueryable<ProductMinifiedOutDto> GetMostRated(int count);
        IQueryable<ProductMinifiedOutDto> GetAll();
        ProductDetailedOutDto GetDetails(int id);
    }
}