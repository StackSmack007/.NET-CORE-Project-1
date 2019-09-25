namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Orders;
    using System.Collections.Generic;

    public interface IOrderService
    {
        void Add(ICollection<PurchaseItemDto> basket, int productId, int ammount);
        void Subtract(List<PurchaseItemDto> basket, int productId, int ammount);
        ICollection<ProductWarranty> GetMyWarranties(string userId);
        ICollection<OrderOutMinifiedDto> GetMyOrders(string userId);
    }
}
