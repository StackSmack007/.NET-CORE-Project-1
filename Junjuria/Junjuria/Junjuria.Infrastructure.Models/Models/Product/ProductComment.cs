namespace Junjuria.Infrastructure.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ProductComment : BaseEntity<int>
    {
        public ProductComment()
        {
            UsersAttitude = new HashSet<CommentSympathy>();
        }
        
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        [Required]
        public string AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        public virtual AppUser Author { get; set; }

        [Required, MaxLength(10240)]
        public string Comment { get; set; }
        
        public virtual ICollection<CommentSympathy> UsersAttitude { get; set; }
    }
}