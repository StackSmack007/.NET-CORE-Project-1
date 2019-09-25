namespace Junjuria.DataTransferObjects.Products
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public  class MyCommentedProductsDto
    {
        public int Id { get; set; }

        [Required, StringLength(maximumLength: 128, MinimumLength = 16)]
        public string Name { get; set; }

        public decimal DiscountedPrice { get; set; }

        public int ComentsCount { get; set; }

        public int MyCommentCount { get; set; }

        public DateTime LastCommentedDate { get; set; }

        public string LastComment { get; set; }

    }
}
