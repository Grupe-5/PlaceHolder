using GP3.Common.Entities;

namespace GP3.Common.Repositories
{
    public interface IIntegrationRepository
    {
        public Task AddIntegrationAsync(IntegrationCallback integration);
        public Task DeleteIntegrationAsync(IntegrationCallback integration);
        public Task<IntegrationCallback?> GetIntegrationAsync(int id);
        public Task<List<IntegrationCallback>> GetIntegrationsAsync();
        public Task<List<IntegrationCallback>> GetIntegrationsByUserAsync(string user);
    }
}
