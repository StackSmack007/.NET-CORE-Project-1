namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.Collections.Generic;

    public class ProductCommentDtoOut : IMapFrom<ProductComment>
    {
        public string AuthorUserName { get; set; }

        public string Comment { get; set; }

        public virtual ICollection<ProductCommentSympathyDtoOut> UsersAttitude { get; set; }
    }


}