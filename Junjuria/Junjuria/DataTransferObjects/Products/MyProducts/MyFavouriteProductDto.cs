using Junjuria.Common.Interfaces.AutoMapper;
namespace Junjuria.DataTransferObjects.Products.MyProducts
{
    using Junjuria.Infrastructure.Models;
    public class MyFavouriteProductDto : MyBaseProduct, IMapFrom<Product>
    {
        public string MainPicURL { get; set; }
        public uint Quantity { get; set; }
    }
}