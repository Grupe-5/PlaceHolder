using GP3.Common.Entities;

namespace GP3.Scraper
{
    public interface IPriceFetcher
    {
        Task<IEnumerable<DayPrice>> GetWeekPricesAsync(DateTime date);
        Task<IEnumerable<DayPrice>> GetWeekPricesAsync(long daysSinceUnixEpoch);
    }
}
