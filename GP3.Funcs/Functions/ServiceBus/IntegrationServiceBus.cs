using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Common.Extensions;
using GP3.Common.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.DuplicateRequestCollapser;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GP3.Funcs.Functions.ServiceBus
{
    public class IntegrationServiceBus
    {

        private readonly HttpClient _client;
        private readonly IIntegrationRepository _integrations;
        private readonly ILogger<IntegrationServiceBus> _logger;
        public IntegrationServiceBus (HttpClient client, IIntegrationRepository integrations, ILogger<IntegrationServiceBus> logger)
        {
            _client = client;
            _integrations = integrations;
            _logger = logger;
        }

        private async Task SendRequest(IntegrationCallback integration, CancellationToken cToken)
        {
            try
            {
                await _client.PostAsJsonAsync(integration.CallbackUrl, integration.CallbackReason, cToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Exception when sending webhook: {e.Message}");
            }
        }

        [FunctionName("IntegrationFunc")]
        public async Task Run([ServiceBusTrigger(ConnStrings.IntegrationQ, Connection = ConnStrings.IntegrationQConn)] string reasonStr, CancellationToken cToken)
        {
            if (!Enum.TryParse<IntegrationCallbackReason>(reasonStr, out var reason))
            {
                _logger.LogError($"Invalid callback reason ({reasonStr})!");
                return;
            }

            var integrations = (await _integrations.GetIntegrationsAsync()).Where(i => i.CallbackReason == reason);
            _logger.LogInformation($"Calling {integrations.Count()} {reason} hooks");
            var cachePolicy = AsyncRequestCollapserPolicy.Create();
            await integrations.ForEachAsync(integration => cachePolicy.ExecuteAsync(
                    (_, token) => SendRequest(integration, token),
                    new Context(integration.CallbackUrl),
                    cToken));
            _logger.LogInformation("Finished sending requests");
        }
    }
}
