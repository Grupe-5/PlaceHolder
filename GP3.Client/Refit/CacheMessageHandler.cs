using GP3.Client.Services;
using Microsoft.Extensions.Logging;
using MonkeyCache;
using System.Net;

namespace GP3.Client.Refit
{
    /* Fallback to cache if device is not connected to internet. */
    public class CacheMessageHandler : DelegatingHandler
    {
        private readonly IConnectivityService _connectivity;
        private readonly IBarrel _barrel;
        private readonly ILogger<CacheMessageHandler> _logger;
        public CacheMessageHandler(IConnectivityService connectivity, IBarrel barrel, ILogger<CacheMessageHandler> logger)
        {
            _connectivity = connectivity;
            _barrel = barrel;
            _logger = logger;
        }

        protected override async Task <HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
        {
            if (!request.Properties.TryGetValue(CacheAttribute.Key, out object? cacheObj))
            {
                /* Continue with request as normal, if cache attribute isn't there */
                _logger.LogInformation($"Request for {request.RequestUri} not cacheable");
                return await base.SendAsync(request, cancelToken);
            }

            /* TODO: Attribute always fails? */

            var cacheSettings = cacheObj as CacheAttribute;
            var barrelKey = request.RequestUri.ToString();

            /* TODO: Add request folding */

            if (!_connectivity.IsConnected())
            {
                _logger.LogInformation("Not connected, returning stale from cache");
                if (_barrel.Exists(barrelKey))
                    return _barrel.Get<HttpResponseMessage>(barrelKey);

                _logger.LogError("No connection and not in cache - throwing");
                throw new WebException("No internet connection!");
            }

            if (!_barrel.IsExpired(barrelKey))
            {
                _logger.LogInformation("Un-expired entry in cache, sending");
                return _barrel.Get<HttpResponseMessage>(barrelKey);
            }
            else if (cacheSettings.BackgroundUpdate && _barrel.Exists(barrelKey))
            {
                /* Return stale entry and complete response in the background */
                /* TODO: This is bad idea (?) Remove later. */
                _logger.LogInformation("Returning stale and updating in background");
                _ = base.SendAsync(request, cancelToken).ContinueWith(a => _barrel.Add(key: barrelKey, data: a.Result, expireIn: cacheSettings.Duration), cancelToken).ConfigureAwait(false);
                return _barrel.Get<HttpResponseMessage>(barrelKey);
            }

            var response = await base.SendAsync(request, cancelToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Adding new info to cache");
                _barrel.Add(key: barrelKey, data: response, expireIn: cacheSettings.Duration);
            }

            return response;
        }
    }
}
