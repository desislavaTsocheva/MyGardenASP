namespace MyGardenWEB.Data
{
    public class Order
    {
        public int Id { get; set; }

        public int ProductsId {  get; set; } //M:1
        public Product Products { get; set; }    

        public string ClientsId {  get; set; }  //M:1
        public Client Clients { get; set; } 

        public int Quantity {  get; set; }
        public DateTime RegisterOn { get; } = DateTime.Now;
       // public int OrderDetailsId { get; set; }
       // public OrderDetail OrderDetails { get; set; }



    }
}