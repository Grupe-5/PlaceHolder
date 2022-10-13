namespace Common
{
    public interface IFetcher
    {
        Task<DayPrices?> GetDayPricesAsync(DateTime date);
        Task<IList<DayPrices>> GetDayPricesAsync(DateTime begin, DateTime end);
    }
}
