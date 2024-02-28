using MyGardenWEB.Data;
using System.ComponentModel.DataAnnotations;

namespace MyGardenWEB.Models
{
    public class ProductsVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is requared!")]
        public string BulgarianName { get; set; }
        [Required(ErrorMessage = "This field is requared!")]
        public string LatinName { get; set; }
        [Required(ErrorMessage = "This field is requared!")]
        public decimal Size { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "This field is requared!")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "This field is requared!")]
        public Category Categories { get; set; }
        [Required(ErrorMessage = "Избери снимка от компютъра си!")]
        public List<IFormFile> ImagePath { get; set; }
    }
}
