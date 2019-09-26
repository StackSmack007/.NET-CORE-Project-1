namespace Junjuria.Infrastructure.Models.Models.Orders
{
using Junjuria.Common.Interfaces.AutoMapper;
    public class ProductQuantityDto:IMapFrom<Product>
    {
        public int Id { get; set; }
        public uint Quantity { get; set; }
    }
}