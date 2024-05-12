using Microsoft.EntityFrameworkCore;

namespace Stock.API.Contexts
{
    public class StockAPIDbContext : DbContext
    {
        public StockAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Entities.Stock> Stocks { get; set; }
    }
}
