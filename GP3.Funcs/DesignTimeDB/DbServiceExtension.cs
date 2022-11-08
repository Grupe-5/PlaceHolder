using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GP3.Funcs.DesignTimeDB
{
    public static class DbServiceExtension
    {
        public static IServiceCollection UseMySqlMig<T>(this IServiceCollection services, string connectionString, string assembly) where T : DbContext
            => services.AddDbContext<T>(o => o.UseMySqlMig<T>(connectionString, assembly));
        public static DbContextOptionsBuilder UseMySqlMig<T>(this DbContextOptionsBuilder builder, string connectionString, string assembly) where T : DbContext
            => builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), c => c.MigrationsAssembly(assembly));
    }
}
