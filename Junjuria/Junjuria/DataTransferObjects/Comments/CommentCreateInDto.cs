namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.ComponentModel.DataAnnotations;

    public class CommentCreateInDto:IMapTo<ProductComment>
    {
        public int ProductId { get; set; }

        [Required, MaxLength(10240)]
        public string Comment { get; set; }
    }
}