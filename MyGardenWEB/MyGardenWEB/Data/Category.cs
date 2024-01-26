using System.ComponentModel.DataAnnotations;

namespace MyGardenWEB.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Description { get; set; }
        public DateTime RegisterOn { get; set; }= DateTime.Now; 
         
        //1:M
        public ICollection<Product> Products { get; set; }  
    }
}