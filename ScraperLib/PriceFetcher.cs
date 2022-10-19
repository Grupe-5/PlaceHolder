using Common;
using Nito.AsyncEx;
using Nito.Disposables.Internals;
using PuppeteerSharp;
using System.Globalization;

namespace ScraperLib
{
    /* TODO: Dependency injection for whole scraper */
    public class PriceFetcher : IFetcher
    {
        private readonly Lazy<PriceScraper> _priceScraper = new(() => new PriceScraper());

        public PriceFetcher() {}

        public async Task<IEnumerable<DayPrices>> GetWeekPricesAsync(DateTime date)
        {
            var scraper = _priceScraper.Value;
            return await scraper.FetchWeekPrices(date);
        }
    }
}
