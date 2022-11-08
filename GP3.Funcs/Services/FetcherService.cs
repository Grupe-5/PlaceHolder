using GP3.Common.Entities;
using GP3.Scraper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GP3.Funcs.Services
{
    public class FetcherService
    {
        private readonly IPriceFetcher _fetcher;
        private readonly ILogger<FetcherService> _logger;
        public FetcherService (IPriceFetcher fetcher, ILogger<FetcherService> logger)
        {
            _fetcher = fetcher;
            _logger = logger;
        }
        public async Task<IEnumerable<DayPrice>> FetchDates(ICollection<long> wantedDays)
        {
            Dictionary<long, DayPrice> newlyFetched = new();
            while (true)
            {
                var prevMissing = wantedDays.Count;
                _logger.LogInformation("Fetching {date}", DateTimeExtension.FromUnixEpoch(wantedDays.First()).ToString("yyyy-MM-dd"));
                var fetched = await _fetcher.GetWeekPricesAsync(wantedDays.First());
                foreach (var day in fetched)
                {
                    if (wantedDays.Contains(day.DaysSinceUnixEpoch))
                    {
                        newlyFetched.TryAdd(day.DaysSinceUnixEpoch, day);
                    }
                }

                wantedDays = wantedDays.Where(i => !newlyFetched.ContainsKey(i)).ToHashSet();
                if (prevMissing == wantedDays.Count || !wantedDays.Any())
                {
                    break;
                }
            }

            if (wantedDays.Any())
            {
                _logger.LogWarning("Unable to fetch: {date}", string.Join(',', wantedDays.Select(i => i.ToString("yyyy-MM-dd")).ToArray()));
            }

            return newlyFetched.Values;
        }
    }
}
