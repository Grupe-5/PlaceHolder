using GP3.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GP3.Common.DB
{
    public class DayPriceDbContext : DbContext
    {
        public DayPriceDbContext(DbContextOptions<DayPriceDbContext> options) : base(options)
        {

        }

        public DbSet<DayPrice> DayPrices => Set<DayPrice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DayPrice>()
                .HasKey(e => new { e.DaysSinceUnixEpoch });
            modelBuilder.Entity<DayPrice>()
                .Property(e => e.HourlyPrices)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<double[]>(v, (JsonSerializerOptions?)null) ?? new double[0]);
        }
    }
}
