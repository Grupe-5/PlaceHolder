using GP3.Common.Entities;
using MonkeyCache;

namespace GP3.Client.Refit
{
    internal class CachedHistoryApi : IHistoryApi
    {
        private static readonly TimeSpan infrequentCache = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan frequentCache = TimeSpan.FromSeconds(30);

        private const string barrelPrefix = "cachedHistoryApi";
        private const string barrelRegistered = "registered";

        private readonly IHistoryApi _api;
        private readonly IBarrel _barrel;
        public CachedHistoryApi(IHistoryApi api, IBarrel barrel)
        {
            _api = api;
            _barrel = barrel;
        }

        public async Task<double> GetCurrentDraw()
        {
            var key = barrelPrefix + "currentDraw";
            if (!_barrel.IsExpired(key))
            {
                return _barrel.Get<double>(key);
            }

            var ans = await _api.GetCurrentDraw();
            _barrel.Add(key: key, data: ans, expireIn: frequentCache);
            return ans;
        }

        public async Task<double> GetDailyUsage()
        {
            var key = barrelPrefix + "dailyUsage";
            if (!_barrel.IsExpired(key))
            {
                return _barrel.Get<double>(key);
            }

            var ans = await _api.GetDailyUsage();
            _barrel.Add(key: key, data: ans, expireIn: infrequentCache);
            return ans;
        }

        public async Task<double> GetMonthlyUsage()
        {
            var key = barrelPrefix + "monthlyUsage";
            if (!_barrel.IsExpired(key))
            {
                return _barrel.Get<double>(key);
            }

            var ans = await _api.GetMonthlyUsage();
            _barrel.Add(key: key, data: ans, expireIn: infrequentCache);
            return ans;
        }

        public async Task<bool> ProviderIsRegistered()
        {
            var key = barrelPrefix + barrelRegistered;
            if (!_barrel.IsExpired(key))
            {
                return _barrel.Get<bool>(key);
            }

            var ans = await _api.ProviderIsRegistered();
            _barrel.Add(key: key, data: ans, expireIn: infrequentCache);
            return ans;
        }

        public async Task RegisterProvider(HistoryRegistration registration)
        {
            await _api.RegisterProvider(registration);
            _barrel.Add(key: barrelPrefix + barrelRegistered, data: true, expireIn: infrequentCache);
        }

        public async Task UnregisterProvider()
        {
            await _api.UnregisterProvider();
            _barrel.Empty(barrelPrefix + barrelRegistered);
        }
    }
}
