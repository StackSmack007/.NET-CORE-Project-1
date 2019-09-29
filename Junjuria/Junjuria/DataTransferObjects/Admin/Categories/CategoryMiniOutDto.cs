using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;

namespace Junjuria.DataTransferObjects.Admin.Categories
{
    public class CategoryMiniOutDto : IMapFrom<Category>
    {
        public int Id { get; set; }


        public string Title { get; set; }
    }
}