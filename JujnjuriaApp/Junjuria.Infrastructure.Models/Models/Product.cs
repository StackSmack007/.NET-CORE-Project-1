namespace Junjuria.Infrastructure.Models.Models
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Infrastructure.Models.Models.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Product : BaseEntity<int>
    {
        public Product()
        {
            Votes = new HashSet<ProductVote>();
            ProductPictures = new HashSet<ProductPicture>();
        }

        public Grade Grade => (Grade)((int)Math.Round((double)Votes.Sum(x => (int)x.Grade) / Votes.Count()));

        [Required, StringLength(maximumLength: 128, MinimumLength = 16)]
        public string Name { get; set; }

        [Required, StringLength(maximumLength: 10240, MinimumLength = 16)]
        public string Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Range(0, 1)]
        public double? Discount { get; set; }

        public uint Quantity { get; set; }

        [Url]
        public string ReviewURL { get; set; }

        [Url]
        public string MainPicURL { get; set; }

        public uint? MonthsWarranty { get; set; }

        public virtual ICollection<ProductVote> Votes { get; set; }
        public virtual ICollection<ProductPicture> ProductPictures { get; set; }
    }
}