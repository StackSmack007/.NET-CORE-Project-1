namespace Junjuria.Infrastructure.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            ProductVotes = new HashSet<ProductVote>();
            CommentSympaties = new HashSet<CommentSympathy>();
            ProductComments = new HashSet<ProductComment>();

            Orders = new HashSet<Order>();
            FavouriteProducts = new HashSet<UserFavouriteProduct>();
            IsDeleted = false;
        }

        [Required, MaxLength(256)]
        public string Address { get; set; }

        [MaxLength(64)]
        public string FirstName { get; set; }

        [MaxLength(64)]
        public string LastName { get; set; }

        [Required, MaxLength(32)]
        public string Town { get; set; }

        public virtual ICollection<ProductVote> ProductVotes { get; set; }

        public virtual ICollection<CommentSympathy> CommentSympaties { get; set; }

        public virtual ICollection<ProductComment> ProductComments { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<UserFavouriteProduct> FavouriteProducts { get; set; }
    }
}