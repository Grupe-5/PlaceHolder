using GP3.Common.Entities;
using GP3.Common.Repositories;
using GP3.Scraper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Funcs.Services
{
    public class PriceService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PriceService> _logger;

        public PriceService(IServiceProvider services, ILogger<PriceService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task<IEnumerable<DayPrice>> GetMissingDates()
        {
            using var serviceScope = _services.CreateScope();
            var priceRepository = serviceScope.ServiceProvider.GetRequiredService<IDayPriceRepository>();
            var fetcherService = serviceScope.ServiceProvider.GetRequiredService<FetcherService>();

            var latestDate = DateTime.Today.DaysSinceUnixEpoch();
            if (FetcherTime.IsTommorowDataAvailable())
            {
                latestDate = DateTime.Today.AddDays(1).DaysSinceUnixEpoch();
            }

            var wantedDays = Enumerable
                .Range(0, (int)(latestDate - FetcherTime.MinDate.DaysSinceUnixEpoch() + 1))
                .Select(i => latestDate - i);

            var availableDays =
                priceRepository
                .GetBetween(FetcherTime.MinDate.DaysSinceUnixEpoch(), latestDate)
                .Select(i => i.DaysSinceUnixEpoch)
                .ToHashSet();

            var missingDays = wantedDays
                .Where(i => !availableDays.Contains(i))
                .ToHashSet();

            if (!missingDays.Any())
            {
                _logger.LogInformation("All available dates present");
                return Enumerable.Empty<DayPrice>();
            }
            else
            {
                _logger.LogInformation("Missing dates: {date}", string.Join(',', missingDays.Select(i => DateTimeExtension.FromUnixEpoch(i).ToString("yyyy-MM-dd")).ToArray()));
            }

            var dates = await fetcherService.FetchDates(missingDays);
            await priceRepository.AddMultipleAsync(dates);
            return dates;
        }
    }
}
