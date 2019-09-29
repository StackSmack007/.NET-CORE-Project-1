namespace Junjuria.DataTransferObjects.Admin.Categories
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.ComponentModel.DataAnnotations;
    public class CategoryInDto : IMapTo<Category>
    {
        public int? CategoryId { get; set; }

        [Required, MaxLength(32)]
        public string Title { get; set; }

        [Required, MaxLength(128)]
        public string Description { get; set; }
    }
}