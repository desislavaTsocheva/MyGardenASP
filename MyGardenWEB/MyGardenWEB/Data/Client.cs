using Microsoft.AspNetCore.Identity;

namespace MyGardenWEB.Data
{
    public class Client : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Description { get; set; }
        public DateTime RegisterOn { get; set; } = DateTime.Now;
        public ICollection<Order> Orders { get; set; }

    }
}