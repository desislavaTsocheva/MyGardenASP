using System.ComponentModel.DataAnnotations.Schema;

namespace MyGardenWEB.Data
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public string ClientsId { get; set; }
        public Client Clients { get; set; }
        public int ProductsId { get; set; } //M:1
        public Product Products { get; set; }
        public DateTime OrderedOn { get; set; }
        public int Quantity { get; set; }
        public bool Final { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        
    }
}
