using GP3.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
