using Common;
using Nito.AsyncEx;

namespace ScraperLib
{
    using ICacheDictionary = IDictionary<long, DayPrices>;
    using CacheDictionary = Dictionary<long, DayPrices>;
    internal sealed class PriceCache
    {
        private readonly String _cachePath;
        private readonly AsyncLazy<ICacheDictionary> _cache;

        public PriceCache(String cachePath)
        {
            _cachePath = cachePath;
            _cache = new AsyncLazy<ICacheDictionary>(async () =>
            {
                var loaded = await CompressedSerializer<ICacheDictionary>.LoadFileAsync(_cachePath);
                return loaded ?? new CacheDictionary();
            });
        }

        public async Task<DayPrices?> SearchCache(DateTime date)
        {
            var cache = await _cache;
            cache.TryGetValue(date.DaysSinceUnixEpoch(), out DayPrices? dayPrices);
            return dayPrices;
        }

        public async Task PopulateCache(IEnumerable<DayPrices> pricesToCache)
        {
            var cache = await _cache;
            int newEntries = 0;
            foreach (var day in pricesToCache)
            {
                if (cache.TryAdd(day.DaysSinceUnixEpoch, day))
                {
                    newEntries++;
                }
                else if (cache[day.DaysSinceUnixEpoch].CompareTo(day) != 0)
                {
                    cache[day.DaysSinceUnixEpoch] = day;
                    newEntries++;
                }
            }

            if (newEntries != 0)
            {
                await CompressedSerializer<ICacheDictionary>.WriteFileAsync(_cachePath, cache);
            }
        }
    }
}
