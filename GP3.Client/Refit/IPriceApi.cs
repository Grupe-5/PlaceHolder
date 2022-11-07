using Refit;
using GP3.Common.Entities;
using GP3.Common.Constants;

namespace GP3.Client.Refit
{
    [Headers("Authorization: Bearer")]
    public interface IPriceApi
    {
        [Get("/" + Routes.GetPrice)]
        Task<DayPrice> GetPriceAsync(string date);
    }
}
