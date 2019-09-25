namespace Junjuria.DataTransferObjects.Products.MyProducts
{
using Junjuria.Infrastructure.Models.Enumerations;
    public class MyRatedProductDto : MyBaseProduct
    {
        public Grade GradeTotal { get; set; }
        public Grade MyGrade { get; set; }
    }
}