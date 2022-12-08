using GP3.Client.Models;
using GP3.Common.Constants;
using GP3.Common.Entities;
using Refit;

namespace GP3.Client.Refit
{
    [Headers("Authorization: Bearer")]
    public interface IIntegrationApi
    {
        [Get("/" + Routes.Integration)]
        Task<IEnumerable<IntegrationCallback>> GetIntegrationsAsync();

        [Post("/" + Routes.Integration)]
        Task<IntegrationCallback> AddIntegrationAsync(IntegrationCallback integration);

        [Delete("/" + Routes.Integration)]
        Task RemoveIntegrationAsync(IntegrationCallback integration);
    }

    public static class IntegrationApiExtensions
    {
        public static async Task<IntegrationCallback> AddIntegrationAsync(this IIntegrationApi api, string callbackUrl, IntegrationCallbackReason reason)
        {
            return await api.AddIntegrationAsync(new IntegrationCallback(reason, callbackUrl));
        }

    }
}
