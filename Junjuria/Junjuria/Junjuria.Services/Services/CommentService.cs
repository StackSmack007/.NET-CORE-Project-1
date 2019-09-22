namespace Junjuria.Services.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    public class CommentService : ICommentService
    {
        private readonly IRepository<ProductComment> commentsRepository;
        private readonly IRepository<Product> productRepository;
        private readonly IMapper mapper;

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

        public IQueryable<ProductComment> Get()
        {
            throw new System.NotImplementedException();
        }
        public async Task AddCommentAsync(CommentCreateInDto dto, AppUser user)
        {
            bool userComentingAgain = productRepository.All().Single(x => x.Id == dto.ProductId).ProductComments.OrderBy(x => x.Id).Last().Author == user;
            if (userComentingAgain) throw new InvalidOperationException("Not allowed to post 2 coments one after another!");
            var comment = mapper.Map<ProductComment>(dto);
            comment.DateOfCreation = DateTime.UtcNow;
            comment.Author = user;
            await commentsRepository.AddAssync(comment);
            await commentsRepository.SaveChangesAsync();
        }
    }
}
