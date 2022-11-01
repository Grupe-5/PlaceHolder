using Common;
using Common.DB;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScraperLib;

[assembly: FunctionsStartup(typeof(ScrapeFunc.Startup))]
namespace ScrapeFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var conf = builder.GetContext().Configuration;

            builder.Services.AddScoped<IPriceBrowser, PriceBrowser>();
            builder.Services.AddScoped<IPricePage, PricePage>();
            builder.Services.AddScoped<IFetcher, PriceFetcher>();

            var dbStr = conf.GetConnectionString("SqlServer");
            builder.Services.AddDbContext<DayPricesDbContext>(o => o
                .UseMySql(dbStr, ServerVersion.AutoDetect(dbStr))
            );
        }
    }
}
