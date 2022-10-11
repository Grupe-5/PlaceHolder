using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    public class IssueDbContext : DbContext
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; }
    }
}
