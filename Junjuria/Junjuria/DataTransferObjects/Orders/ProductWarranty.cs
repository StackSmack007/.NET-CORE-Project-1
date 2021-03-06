﻿namespace Junjuria.DataTransferObjects.Orders
{
    using Junjuria.DataTransferObjects.Products.MyProducts;
    using System;
    public class ProductWarranty : OrderBaseProduct
    {
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}