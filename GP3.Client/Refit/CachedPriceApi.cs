using GP3.Client.Services;
using GP3.Common.Entities;
using Microsoft.Extensions.Logging;
using MonkeyCache;

namespace GP3.Client.Refit
{
    public class CachedPriceApi : IPriceApi
    {
        private const string barrelPrefix = "PriceAPI";
        private readonly TimeSpan barrelDuration = TimeSpan.FromMinutes(5);
        private readonly IPriceApi _api;
        private readonly IBarrel _barrel;
        private readonly IConnectivityService _connectivity;
        private readonly ILogger<CachedPriceApi> _logger;
        public CachedPriceApi(IPriceApi api, IBarrel barrel, IConnectivityService connectivity, ILogger<CachedPriceApi> logger)
        {
            _api = api;
            _barrel = barrel;
            _logger = logger;
            _connectivity = connectivity;
        }

        public async Task<DayPrice> GetPriceAsync(string date)
        {
            return await _api.GetPriceAsync(date);
        }

        public async Task<double[]> GetPriceOffsetAsync(string date)
        {
            var barrelKey = barrelPrefix + date;

            if (!_connectivity.IsConnected())
            {
                _logger.LogInformation("Not connected, returning stale from cache");
                if (_barrel.Exists(barrelKey))
                    return _barrel.Get<double[]>(barrelKey);

                _logger.LogError("No connection and not in cache!");
                return Enumerable.Range(0, 24).Select(i => 0.0).ToArray();
            }

            if (!_barrel.IsExpired(barrelKey))
            {
                _logger.LogInformation("Un-expired entry in cache, sending");
                return _barrel.Get<double[]>(barrelKey);
            }

            var response = await _api.GetPriceOffsetAsync(date);
            _logger.LogInformation("Adding new info to cache");
            _barrel.Add(key: barrelKey, data: response, expireIn: barrelDuration);
            return response;
        }
    }
}
