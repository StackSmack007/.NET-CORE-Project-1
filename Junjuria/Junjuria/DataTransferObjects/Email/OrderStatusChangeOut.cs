namespace Junjuria.DataTransferObjects.Email
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;
    public class OrderStatusChangeOut
    {
        public string OrderId { get; set; }
        public decimal Value { get; set; }
        public DateTime OrderDateTime { get; set; }
        public Status PreviousStatus { get; set; }
        public Status CurrentStatus { get; set; }
    }
}