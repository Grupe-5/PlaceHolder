using Common;
using Nito.AsyncEx;

namespace ScraperLib
{
    public class PriceFetcher : IFetcher
    {
        private readonly Lazy<PriceCache> priceCache = new(() => new PriceCache("./price-cache.bin"));
        private readonly Lazy<PriceScraper> scraper = new(() => new PriceScraper());

        public PriceFetcher() { }

        private async Task<DayPrices?> ScrapeMissing(DateTime date)
        {
            var prices = await scraper.Value.FetchPrices(date);
            await priceCache.Value.PopulateCache(prices);
            return await priceCache.Value.SearchCache(date);
        }

        public async Task<DayPrices?> GetDayPricesAsync(DateTime date)
        {
            return await priceCache.Value.SearchCache(date) ?? await ScrapeMissing(date);
        }

        public async Task<IList<DayPrices>> GetDayPricesAsync(DateTime begin, DateTime end)
        {
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
