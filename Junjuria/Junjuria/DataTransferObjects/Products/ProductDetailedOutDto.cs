namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProductDetailedOutDto
    {
        public int Id { get; set; }
        public Grade Grade => Votes.Any() ? (Grade)(int)Math.Round((double)Votes.Sum(x => (int)x.Grade) / Votes.Count()) : Grade.NotRated;
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public uint Quantity { get; set; }

        public string ReviewURL { get; set; }

        public string MainPicURL { get; set; }

        public uint? MonthsWarranty { get; set; }

        public double Weight { get; set; }

        public int ManufacturerId { get; set; }

        public virtual string ManufacturerName { get; set; }

        public virtual string CategoryTitle { get; set; }
        public virtual int CategoryId { get; set; }

        public virtual ICollection<ProductVoteDto> Votes { get; set; }

        public virtual ICollection<string> ProductPictures { get; set; }

        public virtual ICollection<ProductCommentOutDto> ProductComments { get; set; }

        public virtual ICollection<ProductCharacteristicOutDto> Characteristics { get; set; }
    }

    public class ProductVoteDto:IMapFrom<ProductVote>
    {
        public string  VoterUserName { get; set; }

        public Grade Grade { get; set; }
    }
}