using GP3.Common.DB;
using GP3.Common.Entities;

namespace GP3.Common.Repositories
{
    public class DayPriceRepository : IDayPriceRepository
    {
        private readonly DayPriceDbContext _dbContext;

        public DayPriceRepository(DayPriceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DayPrice?> GetDayPriceAsync(DateTime date)
            => await GetDayPriceAsync(date.DaysSinceUnixEpoch());
        public async Task<DayPrice?> GetDayPriceAsync(long daysSinceUnixEpoch)
            => await _dbContext.DayPrices.FindAsync(daysSinceUnixEpoch);

        public async Task AddAsync(DayPrice price)
        {
            await _dbContext.DayPrices.AddAsync(price);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddMultipleAsync(IEnumerable<DayPrice> prices)
        {
            foreach (var price in prices)
            {
                try
                {
                    await _dbContext.DayPrices.AddAsync(price);
                }
                catch { }
            }
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<DayPrice> GetBetween(DateTime start, DateTime end)
            => GetBetween(start.DaysSinceUnixEpoch(), end.DaysSinceUnixEpoch());

        public IEnumerable<DayPrice> GetBetween(long start, long end)
            => _dbContext.DayPrices.Where(i => i.DaysSinceUnixEpoch >= start && i.DaysSinceUnixEpoch <= end);
    }
}
