using GP3.Common.Constants;
using Refit;

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
    }
}
