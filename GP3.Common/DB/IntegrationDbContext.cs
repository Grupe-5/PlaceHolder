using GP3.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace GP3.Common.DB
{
    public class IntegrationDbContext : DbContext
    {
        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options)
        {

        }

        public DbSet<IntegrationCallback> Integrations => Set<IntegrationCallback>();
    }
}
