namespace Junjuria.DataTransferObjects.Admin.Categories
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.Collections.Generic;

    public partial class CategoryManageItemOutDto : CategoryMiniOutDto, IMapFrom<Category>
    {
        public int? CategoryId { get; set; }

        public ICollection<CategoryMiniOutDto> SubCategories { get; set; }
        public ICollection<CategoryProductOutDto> Products { get; set; }
    }
          }