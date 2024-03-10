using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGardenWEB.Data
{
    public class Product
    {
        [Key]
        public int Id { get; set; } //P.K.
        public string BulgarianName { get; set; }
        public string LatinName { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Size { get; set; }
        public string Description { get; set; }
        public string PhotoURL { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public DateTime RegisterOn { get; set; }=DateTime.Now;  
        public int CategoriesId { get; set; }  //F.K
        public Category Categories { get; set; }    //1:M
        public ICollection<Order> Orders { get; set; }
        public ICollection<Photo> Photos { get; set; }
        //[NotMapped]
       // public List<IFormFile> Files { get; set; } //////////


    }
}
