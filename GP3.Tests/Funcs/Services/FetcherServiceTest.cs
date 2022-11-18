using GP3.Common.Entities;
using GP3.Funcs.Services;
using GP3.Scraper;
using Microsoft.Extensions.Logging.Abstractions;

namespace GP3.Tests.Funcs.Services
{
    public class PriceFetcherFake : IPriceFetcher
    {
        private IEnumerable<DayPrice>? nextVal = null;

        public Task<IEnumerable<DayPrice>> GetWeekPricesAsync(DateTime date)
        {
            var temp = nextVal;
            nextVal = null;
            return Task.FromResult(temp ?? Enumerable.Empty<DayPrice>());
        }

        public Task<IEnumerable<DayPrice>> GetWeekPricesAsync(long daysSinceUnixEpoch)
        {
            var temp = nextVal;
            nextVal = null;
            return Task.FromResult(temp ?? Enumerable.Empty<DayPrice>());
        }

        public void NextReturn(IEnumerable<DayPrice> nextReturn)
        {
            this.nextVal = nextReturn;
        }
    }


    public class FetcherServiceTest
    {
        private FetcherService fetcher;
        private PriceFetcherFake priceFetcher;

        [SetUp]
        public void Setup()
        {
            priceFetcher = new PriceFetcherFake();
            fetcher = new FetcherService(priceFetcher, NullLogger<FetcherService>.Instance);
        }

        [Test]
        public async Task GetEverythingAsync()
        {
            HashSet<long> wantedDays = new HashSet<long>();
            wantedDays.Add(new DateTime(2022, 01, 01).DaysSinceUnixEpoch());
            wantedDays.Add(new DateTime(2022, 01, 02).DaysSinceUnixEpoch());

            var prices = Enumerable.Range(0, 24).Select(i => 0.0).ToArray();
            List<DayPrice> dayPrices = new();
            foreach(var day in wantedDays)
            {
                dayPrices.Add(new DayPrice(day, prices));
            }
            priceFetcher.NextReturn(dayPrices);
            var fetched = await fetcher.FetchDates(wantedDays);
            Assert.That(fetched, Is.EquivalentTo(dayPrices));
        }

        [Test]
        public async Task ExitWhenNoNewVals()
        {
            HashSet<long> wantedDays = new HashSet<long>();
            wantedDays.Add(new DateTime(2022, 01, 01).DaysSinceUnixEpoch());
            wantedDays.Add(new DateTime(2022, 01, 02).DaysSinceUnixEpoch());

            var prices = Enumerable.Range(0, 24).Select(i => 0.0).ToArray();
            List<DayPrice> dayPrices = new();
            foreach(var day in wantedDays)
            {
                dayPrices.Add(new DayPrice(day, prices));
            }

            wantedDays.Add(new DateTime(2022, 01, 03).DaysSinceUnixEpoch());
            priceFetcher.NextReturn(dayPrices);

            var fetched = await fetcher.FetchDates(wantedDays);
            Assert.That(fetched, Is.EquivalentTo(dayPrices));
        }
    }
}
