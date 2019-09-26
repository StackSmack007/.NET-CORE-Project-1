namespace Junjuria.DataTransferObjects.Orders
{
using Junjuria.Common.Interfaces.AutoMapper;
   public class PurchaseItemDetailedDto:PurchaseItemDto,IMapFrom<PurchaseItemDto>
    {
        public string MainPicURL { get; set; }
        public uint StockAmmount { get; set; }

        public double Weight { get; set; }
    }
}