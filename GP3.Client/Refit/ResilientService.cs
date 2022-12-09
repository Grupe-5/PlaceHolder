using GP3.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GP3.Client.Refit
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddResilientApi<T>(this IServiceCollection service, string uri, int retryCount, TimeSpan retryWait, TimeSpan timeout) where T : class
        {
            service.AddTransient<AuthMessageHandler>();

            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(retryCount, _ => retryWait);

            AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
                .TimeoutAsync<HttpResponseMessage>(timeout);

            IAsyncPolicy<HttpResponseMessage> refreshPolicy(IServiceProvider provider, HttpRequestMessage request)
            {
                return Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
                    .RetryAsync(1, async (response, retryCount, context) =>
                    {
                        var authService = provider.GetRequiredService<AuthService>();
                        await authService.RefreshTokenAsync();
                    });
            }

            RefitSettings settings = new()
            {
                Buffered = true,
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions(JsonSerializerDefaults.Web))
            };

            service
                .AddRefitClient<T>(settings)
                .AddPolicyHandler(refreshPolicy)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
                .AddHttpMessageHandler<AuthMessageHandler>();

            return service;
        }
    }
}
