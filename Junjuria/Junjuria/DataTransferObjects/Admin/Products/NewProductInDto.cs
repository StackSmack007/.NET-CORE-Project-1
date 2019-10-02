namespace Junjuria.DataTransferObjects.Admin.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class NewProductInDto
    {
        public NewProductInDto()
        {
            ProductPictures = new List<NewProductPictureDto>() ;
            Characteristics = new List<NewProductCharacteristicDto>();
        }

        [Required, StringLength(maximumLength: 128, MinimumLength = 2)]
        public string Name { get; set; }//

        [Required, StringLength(maximumLength: 10240, MinimumLength = 16)]
        public string Description { get; set; }//

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }//

        [Range(0, 1)]
        public decimal Discount { get; set; }//

        public uint Quantity { get; set; }//

        [Url]
        public string ReviewURL { get; set; }

        [Url]
        public string MainPicURL { get; set; }//

        public uint? MonthsWarranty { get; set; }//

        public double Weight { get; set; }//

        public int ManufacturerId { get; set; }//

        public int CategoryId { get; set; }//

        public string ChangeCount { get; set; }
  
        public virtual IList<NewProductPictureDto> ProductPictures { get; set; }

        public virtual IList<NewProductCharacteristicDto> Characteristics { get; set; }
    }
}