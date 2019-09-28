namespace Junjuria.Infrastructure.Data
{
    using Junjuria.Infrastructure.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.IO;

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
                var configuration = new ConfigurationBuilder()
                                                             .SetBasePath(Directory.GetCurrentDirectory())
                                                             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                             .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
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
            builder.Entity<AppUser>().Property(x => x.Id).HasMaxLength(300);
            builder.Entity<AppUser>().Property(x => x.UserName).HasMaxLength(64);
            builder.Entity<ProductVote>().HasKey(x => new { x.UserId, x.ProductId });
            builder.Entity<ProductPicture>().HasKey(x => new { x.ProductId, x.PictureURL });
            builder.Entity<ProductCharacteristic>().HasKey(x => new { x.ProductId, x.Title });
            builder.Entity<CommentSympathy>().HasKey(x => new { x.CommentId, x.SympathiserId });
            builder.Entity<ProductOrder>().HasKey(x => new { x.ProductId, x.OrderId });
            builder.Entity<UserFavouriteProduct>().HasKey(x => new { x.ProductId, x.UserId });
            base.OnModelCreating(builder);
        }
    }
}