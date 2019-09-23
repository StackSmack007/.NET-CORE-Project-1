namespace Junjuria.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    public class OrderService : IOrderService
    {
        private readonly IRepository<Product> products;

        public OrderService(IRepository<Product> products)
        {
            this.products = products;
        }

        public void Add(ICollection<PurchaseItemDto> basket, int productId, int quantity)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (purchase is null)
            {
                basket.Add(new PurchaseItemDto
                {
                    ProductId = productId,
                    Quantity = quantity
                });
                return;
            }
            purchase.Quantity += quantity;
        }





    }
}