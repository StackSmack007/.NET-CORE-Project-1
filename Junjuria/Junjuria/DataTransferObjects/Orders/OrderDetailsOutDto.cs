namespace Junjuria.DataTransferObjects.Orders
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;
    using System.Collections.Generic;
    public class OrderDetailsOutDto : IMapFrom<Order>
    {
        public DateTime DateOfCreation { get; set; }

        public decimal DeliveryFee { get; set; }
        public Status Status { get; set; }

        public decimal TotalPrice { get; set; }

        public double TotalWeight { get; set; }

        public virtual ICollection<ProductInOrderDto> OrderProducts { get; set; }
     }
}