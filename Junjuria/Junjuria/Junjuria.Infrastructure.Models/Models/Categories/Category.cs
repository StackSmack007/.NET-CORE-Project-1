namespace Junjuria.Infrastructure.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Category : BaseEntity<int>
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int? CategoryId { get; set; }

        public Category OuterCategory { get; set; }

        [Required, MaxLength(32)]
        public string Title { get; set; }

        [Required, MaxLength(128)]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}