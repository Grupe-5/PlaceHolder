using Microsoft.EntityFrameworkCore.Design;

using Context = GP3.Common.DB.HistoryRegistrationDbContext;
namespace GP3.Funcs.DesignTimeDB
{
    public class HistoryRegistrationContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            return new Context(DbJsonExtension<Context>.GetOptions());
        }
    }
}
