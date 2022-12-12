using System.Threading.Tasks;

namespace GP3.Funcs.Services.HistoryProviders
{
    public class Perlas : IProvider
    {
        public Task<double> GetCurrentDraw(string token)
        {
            return Task.FromResult(FakeProviderGenerator.GetCurrentDraw());
        }

        public Task<double> GetDailyUsage(string token)
        {
            return Task.FromResult(FakeProviderGenerator.GetDailyUsage());
        }

        public Task<double> GetMonthyUsage(string token)
        {
            return Task.FromResult(FakeProviderGenerator.GetMonthlyUsage());
        }

        public Task<bool> IsValidToken(string token)
        {
            return Task.FromResult(true);
        }
    }
}
