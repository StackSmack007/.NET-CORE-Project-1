namespace Junjuria.DataTransferObjects.Orders
{
using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
    public class ProductQuantity : IMapFrom<Product>
    {
        public int Id { get; set; }
        public uint Quantity { get; set; }
    }
}