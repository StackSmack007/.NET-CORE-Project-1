namespace Junjuria.DataTransferObjects.Orders
{
using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
    public class ProductQuantityDto : IMapFrom<Product>
    {
        public bool IsDeleted { get; set; }
        public int Id { get; set; }
        public uint Quantity { get; set; }
    }
}