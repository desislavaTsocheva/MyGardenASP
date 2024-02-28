using Microsoft.AspNetCore.Mvc.Rendering;
using MyGardenWEB.Data;
using System.ComponentModel.DataAnnotations;

namespace MyGardenWEB.Models
{
    public class OrdersVM
    {
        public int Id { get; set; }

        public int ProductsId { get; set; } //M:1
        public List<SelectListItem> Products { get; set; }

        public string ClientsId { get; set; }  //M:1
        public int Quantity { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Date)]
        public DateTime RegisterOn { get; set; }
    }
}
