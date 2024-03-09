using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGardenWEB.Data
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public int ProductsId { get; set; }
        public Product Products { get; set; }
        public string Url { get; set; }
        public DateTime RegisterOn { get; set; } = DateTime.Now;
        [NotMapped]
        public List<IFormFile> Files { get; set; } 

    }
}
