using GP3.Common.DB;
using GP3.Common.Repositories;
using GP3.Scraper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(GP3.Funcs.Startup))]
namespace GP3.Funcs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var conf = builder.GetContext().Configuration;

            /* TODO: Replace these with scrutor scan */
            builder.Services.AddScoped<IPriceBrowser, PriceBrowser>();
            builder.Services.AddScoped<IPricePage, PricePage>();
            builder.Services.AddScoped<IPriceFetcher, PriceFetcher>();

            builder.Services.AddScoped<IDayPriceRepository, DayPriceRepository>();
            builder.Services.Decorate<IDayPriceRepository, CachedDayPriceRepository>();

            var cacheStr = conf.GetConnectionString("RedisCache");
            builder.Services.AddStackExchangeRedisCache(o => o
                .Configuration = cacheStr
            );

            var dbStr = conf.GetConnectionString("SqlServer");
            builder.Services.AddDbContext<DayPriceDbContext>(o => o
                .UseMySql(dbStr, ServerVersion.AutoDetect(dbStr))
            );

            builder.Services.AddSingleton<IFirebaseTokenProvider, CustomTokenProvider>(provider => new CustomTokenProvider(
                issuer: "https://securetoken.google.com/gp3-auth",
                audience: "gp3-auth")
            );
        }
    }
}
