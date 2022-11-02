using Common;
using Common.DB;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapeFunc
{
    public class ScrapeTrigger
    {
        private static readonly long lastAvailable = DateTime.ParseExact("2021-01-01", "yyyy-MM-dd", null).DaysSinceUnixEpoch();
        private const long newDataHoursNordpool = 13;

        private readonly ILogger<ScrapeTrigger> _logger;
        private readonly IServiceProvider _services;

        public ScrapeTrigger(IServiceProvider services, ILogger<ScrapeTrigger> logger)
        {
            _logger = logger;
            _services = services;
        }

        private static bool IsTommorowDataAvailable()
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime nordPoolRefreshDate = DateTime.SpecifyKind(DateTime.Today.AddHours(newDataHoursNordpool), DateTimeKind.Unspecified);
            DateTimeOffset dto = new DateTimeOffset(nordPoolRefreshDate, tzi.GetUtcOffset(nordPoolRefreshDate));
            return DateTime.UtcNow >= dto.UtcDateTime;
        }

        private async Task LoadMissingData()
        {
            using IServiceScope scope = _services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DayPricesDbContext>();
            var fetcher = scope.ServiceProvider.GetRequiredService<IFetcher>();

            var latestDate = DateTime.Today.DaysSinceUnixEpoch();
            if (IsTommorowDataAvailable())
            {
                latestDate = DateTime.Today.AddDays(1).DaysSinceUnixEpoch();
            }

            var wantedDays = Enumerable
                .Range(0, (int)(latestDate - lastAvailable + 1))
                .Select(i => latestDate - i).ToHashSet();

            var availableDays = dbContext.DayPrices
                .Select(i => i.DaysSinceUnixEpoch)
                .Where(i => i >= lastAvailable && i <= latestDate).ToHashSet();

            var missingDays = wantedDays.Where(i => !availableDays.Contains(i)).Select(i => DateTime.UnixEpoch.AddDays(i).Date).ToHashSet();
            if (!missingDays.Any())
            {
                _logger.LogInformation("All available dates present");
                return;
            }
            else
            {
                _logger.LogInformation("Trying to fetch: {date}", string.Join(',', missingDays.Select(i => i.ToString("yyyy-MM-dd")).ToArray()));
            }

            Dictionary<DateTime, DayPrices> newlyFetched = new();
            while (true)
            {
                var prevMissing = missingDays.Count;
                _logger.LogInformation("Fetching {date}", missingDays.First().ToString("yyyy-MM-dd"));
                var fetched = await fetcher.GetWeekPricesAsync(missingDays.First());
                foreach (var day in fetched)
                {
                    if (missingDays.Contains(day.Date))
                    {
                        newlyFetched.TryAdd(day.Date, day);
                    }
                }

                missingDays = missingDays.Where(i => !newlyFetched.ContainsKey(i)).ToHashSet();
                if (prevMissing == missingDays.Count || !missingDays.Any())
                {
                    break;
                }
            }

            if (missingDays.Any())
            {
                _logger.LogWarning("Giving up on getting: {date}", string.Join(',', missingDays.Select(i => i.ToString("yyyy-MM-dd")).ToArray()));
            }

            await dbContext.DayPrices.AddRangeAsync(newlyFetched.Values);
            await dbContext.SaveChangesAsync();
        }

        [FunctionName("Scrape")]
        public async Task Run([TimerTrigger("0 * * * *")]TimerInfo myTimer)
        {
            _logger.LogInformation($"Running scraper at: {DateTime.Now}");
            await LoadMissingData();
            _logger.LogInformation($"Next run scheduled for: {myTimer.Schedule.GetNextOccurrence(DateTime.Now)}");
        }
    }
}
