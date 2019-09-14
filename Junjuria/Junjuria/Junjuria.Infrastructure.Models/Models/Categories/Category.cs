namespace Junjuria.Infrastructure.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Category : BaseEntity<int>
    {
        public Category()
        {
            Products = new HashSet<Product>();
            SubCategories = new HashSet<Category>();
        }

        public bool IsMainCategory => OuterCategory is null;
        public bool IsBottomCategory => !SubCategories.Any();

        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category OuterCategory { get; set; }

        [Required, MaxLength(32)]
        public string Title { get; set; }

        [Required, MaxLength(128)]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; }
    }
}