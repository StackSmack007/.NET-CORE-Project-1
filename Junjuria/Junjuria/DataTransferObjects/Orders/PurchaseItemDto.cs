namespace Junjuria.DataTransferObjects.Orders
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;


    public class PurchaseItemDto 
    {
        public int ProductId { get; set; }

        public string Name { get; set; }
        public decimal DiscountedPrice { get; set; }

        public int Quantity { get; set; }
    }
}