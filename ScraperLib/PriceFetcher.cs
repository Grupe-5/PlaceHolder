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
        PriceCache priceCache;
        PriceScraper scraper;

        /* TODO: This doesn't get destroyed */
        ~PriceFetcher()
        {
            Task.Run(() => scraper.DestroyFetcher());
        }

        public PriceFetcher()
        {
            priceCache = new PriceCache();
            scraper = Task.Run(() => PriceScraper.CreateAsync()).Result;
        }

        private DayPrices? ScrapeMissing(DateTime date)
        {
            /* TODO: Use optional arg */
            var prices = Task.Run(() => scraper.FetchPrices(date)).Result;
            priceCache.PopulateCache(prices);
            return priceCache.SearchCache(date);
        }

        public DayPrices? GetDayPrices(DateTime date)
        {
            return priceCache.SearchCache(date) ?? ScrapeMissing(date);
        }
        public IList<DayPrices> GetDayPrices(DateTime begin, DateTime end)
        {
            if (end > begin)
            {
                return new List<DayPrices>();
            }

            int dayCount = (int)(end - begin).TotalDays + 1;
            var prices = new List<DayPrices>();
            for (int i = 0; i < dayCount; i++)
            {
                var price = GetDayPrices(begin.AddDays(i));
                if (price != null)
                {
                    prices.Add(price.GetValueOrDefault());
                }
            }

            return prices;
        }
    }
}
