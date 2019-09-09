namespace Junjuria.Infrastructure.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Manufacturer : BaseEntity<int>
    {
        public Manufacturer()
        {
            Products = new HashSet<Product>();
        }

        [Required,MaxLength(128)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Url]
        public string WebAddress { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}