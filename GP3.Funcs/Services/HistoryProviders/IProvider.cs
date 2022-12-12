using System.Threading.Tasks;

namespace GP3.Funcs.Services.HistoryProviders
{
    public interface IProvider
    {
        Task<double> GetCurrentDraw (string token);
        Task<double> GetMonthyUsage (string token);
        Task<double> GetDailyUsage (string token);
        Task<bool> IsValidToken (string token);
    }
}
