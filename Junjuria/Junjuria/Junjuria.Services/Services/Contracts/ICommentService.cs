namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentService
    {
        //IQueryable<ProductComment> All();

        ProductComment CreateComment(CommentCreateInDto dto, AppUser user);
        string GetLastCommentorId(int productId);
        Task<int> SaveCommentAsync(ProductComment comment);
        Task SetUserAttitudeAsync(Attitude value, int commentId, AppUser user);
        Task<int?> GetProduct(int commentId);
    }
}