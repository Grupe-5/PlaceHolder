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

        public async Task<DayPrice?> GetByDateAsync(DateTime date)
        {
            return await GetByUnixDaysAsync(date.DaysSinceUnixEpoch());
        }

        public async Task<DayPrice?> GetByUnixDaysAsync(long daysSinceUnixEpoch)
        {
            return await _dbContext.DayPrices.FindAsync(daysSinceUnixEpoch);
        }

        public async Task AddAsync(DayPrice price)
        {
            await _dbContext.DayPrices.AddAsync(price);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddMultipleAsync(IEnumerable<DayPrice> prices)
        {
            await _dbContext.DayPrices.AddRangeAsync(prices);
            await _dbContext.SaveChangesAsync();
        }
    }
}
