using Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapeFunc
{
    public class ScrapeTrigger
    {
        private readonly long lastAvailable = DateTime.ParseExact("2021-01-01", "yyyy-MM-dd", null).DaysSinceUnixEpoch();
        private readonly long newDataHours = 14;

        private readonly ILogger<ScrapeTrigger> _logger;
        private readonly IFetcher _fetcher;
        private readonly DayPricesDbContext _dbContext;

        public ScrapeTrigger(IFetcher fetcher, ILogger<ScrapeTrigger> logger, DayPricesDbContext dbContext)
        {
            _fetcher = fetcher;
            _logger = logger;
            _dbContext = dbContext;
        }

        private async Task LoadMissingData()
        {
            var latestDate = DateTime.Today.DaysSinceUnixEpoch();
            if (DateTime.Now >= DateTime.Today.AddHours(newDataHours))
            {
                latestDate = DateTime.Today.AddDays(1).DaysSinceUnixEpoch();
            }

            var wantedDays = Enumerable
                .Range(0, (int)(latestDate - lastAvailable + 1))
                .Select(i => latestDate - i).ToHashSet();

            var availableDays = _dbContext.DayPrices
                .Select(i => i.DaysSinceUnixEpoch)
                .Where(i => i >= lastAvailable && i <= latestDate).ToHashSet();

            var missingDays = wantedDays.Where(i => !availableDays.Contains(i)).Select(i => DateTimeOffset.UnixEpoch.AddDays(i).Date).ToHashSet();
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
                var fetched = await _fetcher.GetWeekPricesAsync(missingDays.First());
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

            await _dbContext.DayPrices.AddRangeAsync(newlyFetched.Values);
            await _dbContext.SaveChangesAsync();
        }

        [FunctionName("Scrape")]
        public async Task Run([TimerTrigger("0 14 * * *")]TimerInfo myTimer)
        {
            _logger.LogInformation($"Running scraper at: {DateTime.Now}");
            await LoadMissingData();
            _logger.LogInformation($"Next run scheduled for: {myTimer.Schedule.GetNextOccurrence(DateTime.Now)}");
        }
    }
}
