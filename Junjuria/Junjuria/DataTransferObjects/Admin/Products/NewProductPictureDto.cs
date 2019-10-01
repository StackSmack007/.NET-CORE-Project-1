using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;

namespace Junjuria.DataTransferObjects.Admin.Products
{
    public class NewProductPictureDto:IMapTo<ProductPicture>
    {
        public string PictureURL { get; set; }
        public string PictureDescription { get; set; }
    }
}