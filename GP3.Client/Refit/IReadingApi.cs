using GP3.Common.Constants;
using Refit;
using GP3.Client.Models;

namespace GP3.Client.Refit
{
    public struct Readings
    {
       public int Id { get; }
    }

    [Headers("Authorization: Bearer")]
    public interface IReadingApi
    {
        [Get("/" + Routes.GetReadings)]
        Task<IEnumerable<Readings>> GetReadingsAsync();

        [Post("/" + Routes.AddReading)]
        Task AddReading(Month month, double payedAmount, double usedKwh);

        /* TODO: For this, month reading model must have some unique Id
        [Delete("/" + Routes.RemoveReading)]
        Task RemoveReading();
        */
    }
}
