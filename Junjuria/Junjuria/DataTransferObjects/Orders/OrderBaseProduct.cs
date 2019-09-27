namespace Junjuria.DataTransferObjects.Orders
{
using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
    public class OrderBaseProduct : IMapFrom<Product>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountedPrice { get; set; }
    }
}