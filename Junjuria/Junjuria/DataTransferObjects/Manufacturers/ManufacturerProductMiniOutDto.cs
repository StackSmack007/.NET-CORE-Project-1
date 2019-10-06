namespace Junjuria.DataTransferObjects.Manufacturers
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.DataTransferObjects.Products.MyProducts;
    using Junjuria.Infrastructure.Models;
    public class ManufacturerProductMiniOutDto : MyBaseProduct, IMapFrom<Product>
    {
       public int CategoryId { get; set; }

        public uint Quantity { get; set; }
        public string CategoryTitle { get; set; }
    }
}