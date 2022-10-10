using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScraperLib
{
    public class PriceFetcher : IFetcher
    {
        PriceCache? priceCache;
        PriceScraper? scraper;

        public PriceFetcher() { }
        public async Task InitAsync()
        {
            this.priceCache = new PriceCache();
            this.scraper = await PriceScraper.CreateAsync();
        }

        public async Task DeinitAsync()
        {
            if (priceCache == null || scraper == null)
            {
                throw new NullReferenceException("Did you forget to call initialize?");
            }

            await scraper.DestroyFetcher();
        }

        private async Task<DayPrices?> ScrapeMissing(DateTime date)
        {
            var prices = await scraper.FetchPrices(date);
            priceCache.PopulateCache(prices);
            return priceCache.SearchCache(date);
        }

        public async Task<DayPrices?> GetDayPricesAsync(DateTime date)
        {
            if (priceCache == null || scraper == null)
            {
                throw new NullReferenceException("Did you forget to call initialize?");
            }
            return priceCache.SearchCache(date) ?? await ScrapeMissing(date);
        }
        public async Task<IList<DayPrices>> GetDayPricesAsync(DateTime begin, DateTime end)
        {
            if (priceCache == null || scraper == null)
            {
                throw new NullReferenceException("Did you forget to call initialize?");
            }

            if (end < begin)
            {
                (begin, end) = (end, begin);
            }

            int dayCount = (int)(end - begin).TotalDays + 1;
            var prices = new List<DayPrices>();
            for (int i = 0; i < dayCount; i++)
            {
                var price = await GetDayPricesAsync(end.AddDays(-1 * i));
                if (price != null)
                {
                    prices.Add(price);
                }
            }

            return prices;
        }
    }
}
