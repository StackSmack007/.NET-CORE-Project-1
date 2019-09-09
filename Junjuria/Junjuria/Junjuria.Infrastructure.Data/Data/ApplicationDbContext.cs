namespace Junjuria.Infrastructure.Data
{
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public ApplicationDbContext()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //For Migrations To Happen!
                optionsBuilder.UseSqlServer(GlobalConstants.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Recomendation> Recomendations { get; set; }
        public DbSet<ProductVote> ProductVotes { get; set; }
        public DbSet<ProductPicture> ProductPictures { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<ProductCharacteristic> ProductCharacteristics { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CommentSympathy> CommentSympathies { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductVote>().HasKey(x => new { x.UserId, x.ProductId });
            builder.Entity<ProductPicture>().HasKey(x => new { x.ProductId, x.PictureURL });
            builder.Entity<ProductCharacteristic>().HasKey(x => new { x.ProductId, x.Title });
            builder.Entity<CommentSympathy>().HasKey(x => new { x.SympathiserId, x.CommentId });
            builder.Entity<ProductOrder>().HasKey(x => new { x.ProductId, x.OrderId });

            base.OnModelCreating(builder);
        }
    }
}