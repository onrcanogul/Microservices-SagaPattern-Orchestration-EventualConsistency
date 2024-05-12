using Microsoft.EntityFrameworkCore;
using Order.API.Entities;

namespace Order.API.Contexts
{
    public class OrderAPIDbContext : DbContext
    {
        public OrderAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Entities.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems  { get; set; }
    }
}
