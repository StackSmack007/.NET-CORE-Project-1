namespace Junjuria.DataTransferObjects.Admin.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.Collections.Generic;

    public class EditProductOutDto : NewProductInDto, IMapFrom<Product>
    {
        public EditProductOutDto() : base()
        {
            ProductComments = new List<EditProductCommentDto>();
        }
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public bool VotesAny { get; set; }

        public virtual IList<EditProductCommentDto> ProductComments { get; set; }
    }
}