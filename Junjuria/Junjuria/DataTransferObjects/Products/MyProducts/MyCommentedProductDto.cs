namespace Junjuria.DataTransferObjects.Products.MyProducts
{
    using System;
    public class MyCommentedProductDto:MyBaseProduct
    {
        public int ComentsCount { get; set; }

        public int MyCommentCount { get; set; }

        public DateTime LastCommentedDate { get; set; }

        public string LastComment { get; set; }
    }
}