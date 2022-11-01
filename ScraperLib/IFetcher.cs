namespace Common
{
    public interface IFetcher
    {
        Task<IEnumerable<DayPrices>> GetWeekPricesAsync(DateTime date);
    }
}
