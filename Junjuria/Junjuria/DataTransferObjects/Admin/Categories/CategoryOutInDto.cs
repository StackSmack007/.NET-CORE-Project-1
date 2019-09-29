namespace Junjuria.DataTransferObjects.Admin.Categories
{
using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
    public  class CategoryOutInDto:CategoryInDto,IMapFrom<Category>
    {
        public int Id { get; set; }
    }
}
