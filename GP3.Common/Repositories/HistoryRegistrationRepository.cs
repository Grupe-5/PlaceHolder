using GP3.Common.DB;
using GP3.Common.Entities;

namespace GP3.Common.Repositories
{
    public class HistoryRegistrationRepository : IHistoryRegistrationRepository
    {
        private readonly HistoryRegistrationDbContext _db;
        public HistoryRegistrationRepository(HistoryRegistrationDbContext db)
        {
            _db = db;
        }

        public async Task RegisterProvider(string userId, string token, ProviderSelection provider)
        {
            await _db.AddAsync(new HistoryRegistration(userId, token, provider));
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UserIsRegistered(string userId)
        {
            return (await _db.HistoryRegistrations.FindAsync(userId)) != null;
        }

        public async Task<HistoryRegistration?> GetRegistration(string userId)
        {
            return await _db.HistoryRegistrations.FindAsync(userId);
        }
        public async Task UnregisterProvider(string userId)
        {
            var reg = new HistoryRegistration(userId, "", 0);
            _db.HistoryRegistrations.Attach(reg);
            _db.HistoryRegistrations.Remove(reg);
            await _db.SaveChangesAsync();
        }
    }
}
