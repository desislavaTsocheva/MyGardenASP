using System.ComponentModel.DataAnnotations.Schema;

namespace MyGardenWEB.ViewModel
{
    public class ProductVM
    {
        public int Id { get; set; } //P.K.
        public string BulgarianName { get; set; }
        public string LatinName { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Size { get; set; }
        public string PhotoURL { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public List<string> Files { get; set; }     
    }
}
