using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyGardenWEB.Data;

namespace MyGardenWEB.Data
{
    public class MyGardenDbContext : IdentityDbContext<Client>
    {
        public MyGardenDbContext(DbContextOptions<MyGardenDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Order> Orders { get; set; }
       // public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<MyGardenWEB.Data.OrderDetail> OrderDetail { get; set; } = default!;
        

    }
}
