using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Common.Extensions;
using GP3.Common.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        private async Task SendRequest(IntegrationCallback integration)
        {
            try
            {
                await _client.PostAsJsonAsync(integration.CallbackUrl, integration.CallbackReason);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Exception when sending webhook: {e.Message}");
            }
        }

        [FunctionName("IntegrationFunc")]
        public async Task Run([ServiceBusTrigger(ConnStrings.IntegrationQ, Connection = ConnStrings.IntegrationQConn)] string reasonStr)
        {
            if (!Enum.TryParse<IntegrationCallbackReason>(reasonStr, out var reason))
            {
                _logger.LogError($"Invalid callback reason ({reasonStr})!");
                return;
            }

            /* Make this more 'safe' with polly policies (shorter timeout, less retries)  */
            var integrations = (await _integrations.GetIntegrationsAsync()).Where(i => i.CallbackReason == reason);
            await integrations.ForEachAsync(integration => SendRequest(integration));
        }
    }
}
