using API.Data;
using Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace API.Services
{
    public class FetcherService : BackgroundService
    {
        /* Oldest date that is supported in Nordpool */
        private readonly long lastAvailable = DateTime.ParseExact("2021-01-01", "yyyy-MM-dd", null).DaysSinceUnixEpoch();
        /* Time at which Nordpool uploads tommorows's prices */
        private readonly long newDataHours = 14;

        private readonly ILogger<FetcherService> _logger;
        private readonly IFetcher _fetcher;
        private readonly IServiceScopeFactory _scopeFactory;

        public FetcherService(IFetcher fetcher, ILogger<FetcherService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _fetcher = fetcher;
            _scopeFactory = scopeFactory;
        }

        private async Task PreloadMissingData ()
        {
            using var scope = _scopeFactory.CreateScope();
            _logger.LogInformation("Preloading missing data...");
            var priceContext = scope.ServiceProvider.GetRequiredService<DayPricesDbContext>();

            var latestDate = DateTime.Today.DaysSinceUnixEpoch();
            if (DateTime.Now >= DateTime.Today.AddHours(newDataHours))
            {
                latestDate = DateTime.Today.AddDays(1).DaysSinceUnixEpoch();
            }

            var wantedDays = Enumerable
                .Range(0, (int)(latestDate - lastAvailable + 1))
                .Select(i => latestDate - i).ToHashSet();

            var availableDays = priceContext.DayPrices
                .Select(i => i.DaysSinceUnixEpoch)
                .Where(i => i >= lastAvailable && i <= latestDate).ToHashSet();

            var missingDays = wantedDays.Where(i => !availableDays.Contains(i)).Select(i => DateTimeOffset.UnixEpoch.AddDays(i).Date).ToHashSet();

            Dictionary<DateTime, DayPrices>? newlyFetched = new();
            while (true)
            {
                if (!missingDays.Any())
                {
                    break;
                }

                var prevMissing = missingDays.Count;
                var fetched = await _fetcher.GetWeekPricesAsync(missingDays.First());
                foreach(var day in fetched)
                {
                    if (missingDays.Contains(day.Date))
                    {
                        newlyFetched.TryAdd(day.Date, day);
                    }
                }

                missingDays = missingDays.Where(i => !newlyFetched.ContainsKey(i)).ToHashSet();
                if (prevMissing == missingDays.Count)
                {
                    break;
                }
            }

            if (missingDays.Any())
            {
                _logger.LogWarning("Giving up on getting {date}", string.Join(',', missingDays.Select(i => i.ToString("yyyy-MM-dd")).ToArray()));
            }

            await priceContext.DayPrices.AddRangeAsync(newlyFetched.Values);
            await priceContext.SaveChangesAsync();
            _logger.LogInformation("Finished preloading");
        }

        private async Task FetchTommorowData ()
        {
            using var scope = _scopeFactory.CreateScope();
            var priceContext = scope.ServiceProvider.GetRequiredService<DayPricesDbContext>();
            var tommorow = DateTime.Today.AddDays(1);
            if ((await priceContext.DayPrices.FindAsync(tommorow.DaysSinceUnixEpoch())) != null)
            {
                var fetched = (await _fetcher.GetWeekPricesAsync(tommorow)).Where(i => i.Date == tommorow);
                await priceContext.DayPrices.AddAsync(fetched.First());
                _logger.LogInformation("Succesfully added tommorows price data");
            }
            else
            {
                _logger.LogWarning("DB already contained tommorow's price data!");
            }
        }

        private TimeSpan CalculateNextRun()
        {
            var now = DateTime.Now;
            var todayExec = DateTime.Today.AddHours(newDataHours);
            var tommorowExec = DateTime.Today.AddDays(1).AddHours(newDataHours);

            if (now < DateTime.Today.AddHours(newDataHours))
            {
                return TimeSpan.FromSeconds((todayExec - now).TotalSeconds);
            }
            else
            {
                return TimeSpan.FromSeconds((tommorowExec - now).TotalSeconds);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delayAmount = CalculateNextRun();
                _logger.LogInformation("Executing next fetch at {} (in ~{} hours)", DateTime.Now.Add(delayAmount), delayAmount.TotalHours.ToString("0.00"));
                await Task.Delay(delayAmount, stoppingToken);
                await FetchTommorowData();
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(PreloadMissingData, cancellationToken).Wait(cancellationToken);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync");
            return base.StopAsync(cancellationToken);
        }
    }
}
