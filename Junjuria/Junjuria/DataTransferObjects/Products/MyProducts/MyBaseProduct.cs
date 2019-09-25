namespace Junjuria.DataTransferObjects.Products.MyProducts
{
   public abstract class MyBaseProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountedPrice { get; set; }
    }
}
