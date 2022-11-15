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
        IntegrationCallback AddIntegrationAsync(IntegrationCallback integration);

        [Delete("/" + Routes.Integration)]
        Task RemoveIntegrationAsync(IntegrationCallback integration);
    }
}
