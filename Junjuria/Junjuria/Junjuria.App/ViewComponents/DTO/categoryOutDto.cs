namespace Junjuria.App.ViewComponents.DTO
{
    public class CategoryOutDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int? OuterCategoryId { get; set; }

        public int ProductsCount { get; set; }

        public int SubProductsCount { get; set; }
    }
}