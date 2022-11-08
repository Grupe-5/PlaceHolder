using Microsoft.EntityFrameworkCore.Design;

using Context = GP3.Common.DB.IntegrationDbContext;
namespace GP3.Funcs.DesignTimeDB
{
    public class IntegrationContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            return new Context(DbJsonExtension<Context>.GetOptions());
        }
    }
}
