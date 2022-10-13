using Microsoft.EntityFrameworkCore;
using Common;

namespace API.Data
{
    public class DayPricesDbContext : DbContext
    {
        public DayPricesDbContext(DbContextOptions<DayPricesDbContext> options) : base(options)
        {
        }

        public DbSet<DayPrices> DayPrices => Set<DayPrices>();
    }
}
