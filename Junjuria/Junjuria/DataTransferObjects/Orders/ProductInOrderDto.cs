namespace Junjuria.DataTransferObjects.Orders
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;

    public class ProductInOrderDto:IMapFrom<ProductOrder>
    {
        public decimal CurrentPrice { get; set; }

         public int Quantity { get; set; }

        public virtual OrderBaseProduct Product { get; set; }
    }
}

