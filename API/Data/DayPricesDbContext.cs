using Microsoft.EntityFrameworkCore;
using Common;
using PuppeteerSharp;
using System.Text.Json;

namespace API.Data
{
    public class DayPricesDbContext : DbContext
    {
        public DayPricesDbContext(DbContextOptions<DayPricesDbContext> options) : base(options)
        {
        }

        public DbSet<DayPrices> DayPrices => Set<DayPrices>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DayPrices>()
                .HasKey(e => new { e.DaysSinceUnixEpoch });
            modelBuilder.Entity<DayPrices>()
                .Property(e => e.HourlyPrices)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<double[]>(v, (JsonSerializerOptions?)null));
        }
    }
}
