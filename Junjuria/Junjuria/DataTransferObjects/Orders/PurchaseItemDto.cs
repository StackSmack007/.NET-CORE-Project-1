﻿namespace Junjuria.DataTransferObjects.Orders
{
    public class PurchaseItemDto 
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal DiscountedPrice { get; set; }

        public uint Quantity { get; set; }
    }
}