using GP3.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            modelBuilder.Entity<DayPrice>()
                .HasKey(e => new { e.DaysSinceUnixEpoch });

            modelBuilder.Entity<DayPrice>()
                .Property(e => e.HourlyPrices)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<double[]>(v, (JsonSerializerOptions?)null) ?? new double[0]);

            var priceComparer = new ValueComparer<double[]>(
                (c1, c2) => (c1 != null && c2 != null) && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray());

            modelBuilder.Entity<DayPrice>()
                .Property(e => e.HourlyPrices)
                .Metadata
                .SetValueComparer(priceComparer);
        }
    }
}
