namespace Junjuria.DataTransferObjects.Admin.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.ComponentModel.DataAnnotations;

    public class EditProductCommentDto:IMapFrom<ProductComment>
    {
        public bool IsDeleted { get; set; }

        public int Id { get; set; }

        public string AuthorUserName { get; set; }

        [Required, MaxLength(10240)]
        public string Comment { get; set; }
    }
}