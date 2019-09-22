namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentService
    {
        IQueryable<ProductComment> All();

        IQueryable<ProductComment> Get();
        Task AddCommentAsync(CommentCreateInDto dto, AppUser user);
    }
}
