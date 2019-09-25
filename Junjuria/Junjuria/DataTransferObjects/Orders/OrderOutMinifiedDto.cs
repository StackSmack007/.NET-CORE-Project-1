namespace Junjuria.DataTransferObjects.Orders
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;

    public class OrderOutMinifiedDto : IMapFrom<Order>
    {
        public string Id { get; set; }
        public decimal DeliveryFee  { get; set; }

        public DateTime DateOfCreation { get; set; }
        public Status  Status       { get; set; }

        public decimal TotalPrice   { get; set; }

        public double  TotalWeight  { get; set; }
    }
}