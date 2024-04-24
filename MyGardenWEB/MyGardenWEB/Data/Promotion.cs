using System.ComponentModel;

namespace MyGardenWEB.Data
{
    public class Promotion
    {
        public int Id { get; set; }
        public int ProductsId { get; set; }
        [DisplayName("Промоционален продукт")]
        public Product Products { get; set; }
        [DisplayName("Промоция")]
        public int PromotionPercent { get; set; }
    }
}
