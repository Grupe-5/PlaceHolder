using GP3.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace GP3.Common.DB
{
    public class HistoryRegistrationDbContext : DbContext
    {
        public HistoryRegistrationDbContext(DbContextOptions<HistoryRegistrationDbContext> options) : base(options)
        {

        }

        public DbSet<HistoryRegistration> HistoryRegistrations => Set<HistoryRegistration>();
    }
}
