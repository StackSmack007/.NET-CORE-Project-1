namespace Junjuria.DataTransferObjects.Admin.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System;
    public class NewProductPictureDto :  IComparable<NewProductPictureDto>, IMapFrom<ProductPicture>,IMapTo<ProductPicture>
    {
        public string PictureURL { get; set; }
        public string PictureDescription { get; set; }

        public int CompareTo(NewProductPictureDto other)
        {
            if (PictureURL == other.PictureURL)
            {
                return 0;
            }
            return 1;
        }
    }
}