namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private IRepository<ProductComment> commentsRepository;
        private IRepository<Product> productRepository;
        private IMapper mapper;

        public CommentService(IRepository<ProductComment> commentsRepository, IRepository<Product> productRepository, IMapper mapper)
        {
            this.commentsRepository = commentsRepository;
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public IQueryable<ProductComment> All()
        {
            throw new System.NotImplementedException();
        }

        public ProductComment CreateComment(CommentCreateInDto dto, AppUser user)
        {
            var comment = mapper.Map<ProductComment>(dto);
            comment.DateOfCreation = DateTime.UtcNow;
            comment.Author = user;
            return comment;
        }

        public string GetLastCommentorId(int productId)
        {
            return productRepository.All().Where(x => x.Id == productId)
                                          .SelectMany(x => x.ProductComments)
                                          .Where(x=>!x.IsDeleted)
                                          .OrderBy(x => x.Id)
                                          .Select(x => x.Author.Id)
                                          .LastOrDefault();
        }

        public async Task<int?> GetProduct(int commentId)
        {
           var comment= await commentsRepository.All().FirstOrDefaultAsync(x => x.Id == commentId);
            return comment?.ProductId;
        }

        public async Task<int> SaveCommentAsync(ProductComment comment)
        {
            await commentsRepository.AddAssync(comment);
            return await commentsRepository.SaveChangesAsync();
        }


        public async Task SetUserAttitudeAsync(Attitude value, int commentId, AppUser user)
        {
            var oldVote = await commentsRepository.All().Where(x => x.Id == commentId).SelectMany(x => x.UsersAttitude).FirstOrDefaultAsync(x => x.SympathiserId == user.Id);
            if (oldVote is null)
            {
                ProductComment comment = await GetByIdAsync(commentId);
                comment.UsersAttitude.Add(new CommentSympathy
                {
                    Sympathiser = user,
                    Attitude = value,
                });
            }
            else
            {
                oldVote.Attitude = value;
            }
            await commentsRepository.SaveChangesAsync();
        }

        private async Task<ProductComment> GetByIdAsync(int id)
        {
            return await commentsRepository.All().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}