namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;

    public class ProductCharacteristicOutDto : IMapFrom<ProductCharacteristic>
    {
        public string Title { get; set; }
        public string TextValue { get; set; }
        public double? NumericValue { get; set; }
    }
}