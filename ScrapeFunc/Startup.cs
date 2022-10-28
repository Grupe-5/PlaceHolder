using Common;
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

            builder.Services.AddSingleton<IPriceBrowser, PriceBrowser>();
            builder.Services.AddSingleton<IPricePage, PricePage>();
            builder.Services.AddSingleton<IFetcher, PriceFetcher>();

            var dbStr = conf.GetConnectionString("SqlServer");
            builder.Services.AddDbContext<DayPricesDbContext>(o => o
                .UseMySql(dbStr, ServerVersion.AutoDetect(dbStr))
            );
        }
    }
}
