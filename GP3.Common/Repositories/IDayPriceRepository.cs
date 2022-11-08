using GP3.Common.Entities;

namespace GP3.Common.Repositories
{
    public interface IDayPriceRepository
    {
        Task<DayPrice?> GetDayPriceAsync(long daysSinceUnixEpoch);
        Task<DayPrice?> GetDayPriceAsync(DateTime date);

        IEnumerable<DayPrice> GetBetween(DateTime start, DateTime end);
        IEnumerable<DayPrice> GetBetween(long start, long end);

        Task AddAsync(DayPrice price);
        Task AddMultipleAsync(IEnumerable<DayPrice> prices);
    }
}
