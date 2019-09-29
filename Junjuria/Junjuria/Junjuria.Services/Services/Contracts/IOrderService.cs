namespace Junjuria.Services.Services.Contracts
{
    using Junjuria.DataTransferObjects.Orders;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderService
    {
        void AddProductToBasket(ICollection<PurchaseItemDto> basket, int productId, uint ammount);
        void SubtractProductFromBasket(List<PurchaseItemDto> basket, int productId, uint ammount);
        ICollection<ProductWarranty> GetMyWarranties(string userId);
        ICollection<OrderOutMinifiedDto> GetMyOrders(string userId);
        ICollection<PurchaseItemDetailedDto> GetDetailedPurchaseInfo(ICollection<PurchaseItemDto> purchases);
        void ModifyCountOfProductInBasket(List<PurchaseItemDto> basket, int productId, uint newAmmount);
        bool TryCreateOrder(List<PurchaseItemDto> basket,string userId);
        Task<OrderDetailsOutDto> GetOrderDetails(string id);
    }
}
