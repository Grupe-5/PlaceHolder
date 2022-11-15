using Refit;
using GP3.Common.Entities;
using GP3.Common.Constants;

namespace GP3.Client.Refit
{
    [Headers("Authorization: Bearer")]
    public interface IPriceApi
    {
        [Get("/" + Routes.Price)]
        Task<DayPrice> GetPriceAsync(string date);

        [Get("/" + Routes.PriceOffset)]
        Task<double[]> GetPriceOffsetAsync(string date);
    }

    public static class PriceApiExtensions
    {
        public static async Task<DayPrice> GetPriceAsync(this IPriceApi api, DateTime date)
        {
            return await api.GetPriceAsync(date.ToString("yyyy-MM-dd"));
        }
        public static async Task<DayPrice> GetPriceOffsetAsync(this IPriceApi api, DateTime date)
        {
            var prices = await api.GetPriceOffsetAsync(date.ToUniversalTime().ToString("yyyy-MM-dd:HH"));
            return new DayPrice(date, prices);
        }
    }
}
