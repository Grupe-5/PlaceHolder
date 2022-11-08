using GP3.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GP3.Funcs.DesignTimeDB
{
    public static class DbJsonExtension<TContext> where TContext : DbContext
    {
        public static DbContextOptions<TContext> GetOptions()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseMySqlMig<TContext>(config.GetConnectionString(ConnStrings.SQL), ConnStrings.ContextAssembly);
            return optionsBuilder.Options;
        }
    }
}
