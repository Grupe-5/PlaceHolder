using GP3.Common.Entities;

namespace GP3.Common.Repositories
{
    public interface IHistoryRegistrationRepository
    {
        Task<bool> UserIsRegistered(string userId);
        Task RegisterProvider(string userId, string token, ProviderSelection provider);
        Task<HistoryRegistration?> GetRegistration(string userId);
        Task UnregisterProvider(string userId);
    }
}
