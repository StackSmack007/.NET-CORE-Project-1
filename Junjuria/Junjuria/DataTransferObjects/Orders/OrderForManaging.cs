namespace Junjuria.DataTransferObjects.Orders
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;

    public class OrderForManaging
    {
        public string Id { get; set; }
        public DateTime DateOfCreation { get; set; }
       // public bool IsDeleted { get; set; }
        public string CustomerId { get; set; }
        public string CustomerUserName { get; set; }

        public decimal DeliveryFee { get; set; }

        public Status Status { get; set; }

        public int OrderProductsCount { get; set; }

        public decimal TotalPrice { get; set; }
        public double TotalWeight { get; set; }
    }
}