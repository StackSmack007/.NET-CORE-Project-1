namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Orders;
    using System.Collections.Generic;

    public interface IOrderService
    {
        void Add(ICollection<PurchaseItemDto> basket, int productId, int count);
    }
}
