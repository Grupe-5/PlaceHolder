using GP3.Common.Constants;
using GP3.Common.Entities;
using Refit;

namespace GP3.Client.Refit
{
    [Headers("Authorization: Bearer")]
    public interface IHistoryApi
    {
        [Get("/" + Routes.HistoryCurrentDraw)]
        Task<double> GetCurrentDraw();

        [Get("/" + Routes.HistoryMonthlyUsage)]
        Task<double> GetMonthlyUsage();

        [Get("/" + Routes.HistoryDailyUsage)]
        Task<double> GetDailyUsage();

        [Get("/" + Routes.HistoryIsRegistered)]
        Task<bool> ProviderIsRegistered();

        [Post("/" + Routes.HistoryRegisterProvider)]
        Task RegisterProvider(HistoryRegistration registration);

        [Delete("/" + Routes.HistoryRegisterProvider)]
        Task UnregisterProvider();
    }
}
