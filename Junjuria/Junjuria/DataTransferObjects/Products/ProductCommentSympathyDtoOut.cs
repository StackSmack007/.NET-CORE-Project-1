namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;

    public class ProductCommentSympathyDtoOut:IMapFrom<CommentSympathy>
    {
        public string SympathiserUserName { get; set; }
        public Attitude Attitude { get; set; }
    }


}