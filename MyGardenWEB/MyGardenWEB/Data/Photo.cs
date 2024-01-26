namespace MyGardenWEB.Data
{
    public class Photo
    {
        public int Id { get; set; } 
        public int ProductsId {  get; set; }
        public Product Products { get; set; }
        public string Url {  get; set; }    
        public DateTime RegisterOn {  get; set; }   

    }
}
