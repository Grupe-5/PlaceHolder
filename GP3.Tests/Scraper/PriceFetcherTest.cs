using Castle.Core.Logging;
using GP3.Common.Entities;
using GP3.Scraper;
using Microsoft.Extensions.Logging.Abstractions;

namespace GP3.Tests.Scraper
{
    public class PriceFetcherTest
    {
        static private readonly PriceFetcher fetcher = new PriceFetcher(PricePageTest.CreatePricePage(), NullLogger<PriceFetcher>.Instance);
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task TestFetchPrices()
        {
            var prices = await fetcher.GetWeekPricesAsync(DateTime.Now);
            Assert.That(prices.Count(), Is.EqualTo(8));
        }

        [Test]
        public async Task TestNotFetchInvalid()
        {
            var prices = await fetcher.GetWeekPricesAsync(DateTime.UnixEpoch.DaysSinceUnixEpoch());
            Assert.That(prices.Count(), Is.EqualTo(0));
        }
    }
}
