using GP3.Common.DB;
using GP3.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Common.Repositories
{
    public class IntegrationRepository : IIntegrationRepository
    {
        private readonly IntegrationDbContext _context;

        public IntegrationRepository(IntegrationDbContext context)
        {
            _context = context;
        }

        public async Task AddIntegrationAsync(IntegrationCallback integration)
        {
            await _context.Integrations.AddAsync(integration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteIntegrationAsync(IntegrationCallback integration)
        {
            _context.Integrations.Remove(integration);
            await _context.SaveChangesAsync();
        }

        public async Task<IntegrationCallback?> GetIntegrationAsync(int id)
            => await _context.Integrations.FindAsync(id);

        public async Task<List<IntegrationCallback>> GetIntegrationsAsync()
            => await _context.Integrations.ToListAsync();

        public async Task<List<IntegrationCallback>> GetIntegrationsByUserAsync(string user)
            => await _context.Integrations.Where(i => i.User == user).ToListAsync();
    }
}
