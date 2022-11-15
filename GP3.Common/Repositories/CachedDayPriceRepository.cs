using GP3.Common.Entities;
using GP3.Common.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GP3.Common.Repositories
{
    /* Main purpose is to cache day prices that are added or requested */
    public class CachedDayPriceRepository : IDayPriceRepository
    {
        private readonly IDayPriceRepository _dayPriceRepository;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachedDayPriceRepository> _logger;

        public CachedDayPriceRepository(IDayPriceRepository dayPriceRepository, IDistributedCache cache, ILogger<CachedDayPriceRepository> logger)
        {
            _dayPriceRepository = dayPriceRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task AddAsync(DayPrice price)
        {
            try
            {
                await _cache.SetAsync<DayPrice>(price.DaysSinceUnixEpoch.ToString(), price);
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered cache error: {e.Message}");
            }
            await _dayPriceRepository.AddAsync(price);
        }

        public async Task AddMultipleAsync(IEnumerable<DayPrice> prices)
        {
            foreach (var price in prices)
            {
                try
                {
                    await _cache.SetAsync(price.DaysSinceUnixEpoch.ToString(), price);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Encountered cache error: {e.Message}");
                }
            }

            await _dayPriceRepository.AddMultipleAsync(prices);
        }

        public async Task<DayPrice?> GetDayPriceAsync(DateTime date)
            => await GetDayPriceAsync(date.DaysSinceUnixEpoch());

        public async Task<DayPrice?> GetDayPriceAsync(long daysSinceUnixEpoch)
        {
            DayPrice? val = null;
            var key = daysSinceUnixEpoch.ToString();
            try
            {
                val = await _cache.GetValueAsync<DayPrice>(key);
                if (val != null)
                {
                    await _cache.RefreshAsync(key);
                    return val;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered cache error: {e.Message}");
            }

            val = await _dayPriceRepository.GetDayPriceAsync(daysSinceUnixEpoch);
            if (val != null)
            {
                try
                {
                    await _cache.SetAsync(daysSinceUnixEpoch.ToString(), val);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Encountered cache error: {e.Message}");
                }
            }
            return val;
        }

        public IEnumerable<DayPrice> GetBetween(DateTime start, DateTime end)
            => _dayPriceRepository.GetBetween(start, end);

        public IEnumerable<DayPrice> GetBetween(long start, long end)
            => _dayPriceRepository.GetBetween(start, end);
    }
}
