using GP3.Common.Entities;

namespace GP3.Common.Repositories
{
    public interface IDayPriceRepository
    {
        Task<DayPrice?> GetByUnixDaysAsync(long daysSinceUnixEpoch);
        Task<DayPrice?> GetByDateAsync(DateTime date);

        Task AddAsync(DayPrice price);
        Task AddMultipleAsync(IEnumerable<DayPrice> prices);
    }
}
