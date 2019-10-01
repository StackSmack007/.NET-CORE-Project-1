namespace Junjuria.DataTransferObjects.Admin.Products
{
    public class ProductForManagingOutDto
    {
        public bool IsDeleted { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal DiscountedPrice { get; set; }

        public uint Quantity { get; set; }

        public string CategoryTitle { get; set; }

        public int ProductOrdersPending { get; set; }

        public int OrderedQuantityPending { get; set; }

        public int ProductOrdersTotal { get; set; }

        public int OrderedQuantityTotal { get; set; }

        public int UsersFavouringThisProductCount { get; set; }
    }
}