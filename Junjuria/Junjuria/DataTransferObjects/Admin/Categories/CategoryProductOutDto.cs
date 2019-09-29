namespace Junjuria.DataTransferObjects.Admin.Categories
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;

    public partial class CategoryManageItemOutDto
    {
        public class CategoryProductOutDto : IMapFrom<Product>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }





}