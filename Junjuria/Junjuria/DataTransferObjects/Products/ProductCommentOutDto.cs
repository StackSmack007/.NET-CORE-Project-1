namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System;
    using System.Collections.Generic;

    public class ProductCommentOutDto : IMapFrom<ProductComment>
    {
        public int Id { get; set; }
        public string AuthorUserName { get; set; }

        public string Comment { get; set; }

        public DateTime DateOfCreation { get; set; }

        public virtual ICollection<ProductCommentSympathyOutDto> UsersAttitude { get; set; }
    }
}