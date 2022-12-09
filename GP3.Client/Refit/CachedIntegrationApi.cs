using GP3.Client.Services;
using GP3.Common.Entities;
using Microsoft.Extensions.Logging;
using MonkeyCache;
using System.Text.Json;

namespace GP3.Client.Refit
{
    public class CachedIntegrationApi : IIntegrationApi
    {
        private const string barrelPrefix = "IntegrationAPI";
        private readonly TimeSpan barrelDuration = TimeSpan.FromMinutes(5);
        private readonly IIntegrationApi _api;
        private readonly IBarrel _barrel;
        private readonly IConnectivityService _connectivity;
        private readonly ILogger<CachedIntegrationApi> _logger;
        public CachedIntegrationApi(IIntegrationApi api, IBarrel barrel, IConnectivityService connectivity, ILogger<CachedIntegrationApi> logger)
        {
            _api = api;
            _barrel = barrel;
            _logger = logger;
            _connectivity = connectivity;
            _barrel.Empty(barrelPrefix);
        }

        public async Task<IEnumerable<IntegrationCallback>> GetIntegrationsAsync()
        {
            if (!_connectivity.IsConnected())
            {
                _logger.LogInformation("Not connected, returning stale from cache");
                if (_barrel.Exists(barrelPrefix))
                    return _barrel.Get<IEnumerable<IntegrationCallback>>(barrelPrefix);

                _logger.LogError("No connection and not in cache!");
                return default;
            }

            if (!_barrel.IsExpired(barrelPrefix))
            {
                _logger.LogInformation("Un-expired entry in cache, sending");
                return _barrel.Get<IEnumerable<IntegrationCallback>>(barrelPrefix);
            }

            var response = await _api.GetIntegrationsAsync();
            _logger.LogInformation("Adding new info to cache");
            _barrel.Add(key: barrelPrefix, data: response, expireIn: barrelDuration);
            return response;
        }

        public async Task<IntegrationCallback> AddIntegrationAsync(IntegrationCallback integration)
        {
            if (!_connectivity.IsConnected())
            {
                _logger.LogError("No internet connection!");
                throw new Exception("No internet connection!");
            }

            var ret = await _api.AddIntegrationAsync(integration);
            if (_barrel.Exists(barrelPrefix))
            {
                var cachedIntegrations = _barrel.Get<IEnumerable<IntegrationCallback>>(barrelPrefix)
                    .Where(i => i.Id != integration.Id).Append(ret);
                _barrel.Add(key: barrelPrefix, data: cachedIntegrations, expireIn: barrelDuration);
            }

            integration.Id = ret.Id;
            integration.User = ret.User;
            return ret;
        }

        public async Task RemoveIntegrationAsync(IntegrationCallback integration)
        {
            await _api.RemoveIntegrationAsync(integration);
            if (_barrel.Exists(barrelPrefix))
            {
                var cachedIntegrations = _barrel.Get<IEnumerable<IntegrationCallback>>(barrelPrefix).Where(i => i.Id != integration.Id);
                _barrel.Add(key: barrelPrefix, data: cachedIntegrations, expireIn: barrelDuration);
            }
        }
    }
}
