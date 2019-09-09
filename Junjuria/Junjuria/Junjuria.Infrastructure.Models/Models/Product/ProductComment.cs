namespace Junjuria.Infrastructure.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ProductComment : BaseEntity<int>
    {
        public ProductComment()
        {
            UsersAttitude = new HashSet<CommentSympathy>();
        }

        public string AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        public virtual AppUser Author { get; set; }

        public virtual ICollection<CommentSympathy> UsersAttitude { get; set; }
    }
}