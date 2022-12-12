using GP3.Common.Entities;
using GP3.Common.Repositories;
using GP3.Funcs.Services.HistoryProviders;
using System.Threading.Tasks;

namespace GP3.Funcs.Services
{
    public class ProviderService
    {
        private readonly ProviderFactory _factory;
        private readonly IHistoryRegistrationRepository _repository;
        public ProviderService (ProviderFactory factory, IHistoryRegistrationRepository repository)
        {
            _factory = factory;
            _repository = repository;
        }

        public async Task<double?> GetCurrentDraw (string userId)
        {
            var registration = await _repository.GetRegistration(userId);
            if (registration == null)
            {
                return null;
            }

            return await _factory.GetProvider(registration.Provider)
                    .GetCurrentDraw(registration.Token);
        }

        public async Task<double?> GetMonthyUsage (string userId)
        {
            var registration = await _repository.GetRegistration(userId);
            if (registration == null)
            {
                return null;
            }

            return await _factory.GetProvider(registration.Provider)
                    .GetMonthyUsage(registration.Token);
        }

        public async Task<double?> GetDailyUsage (string userId)
        {
            var registration = await _repository.GetRegistration(userId);
            if (registration == null)
            {
                return null;
            }

            return await _factory.GetProvider(registration.Provider)
                    .GetDailyUsage(registration.Token);
        }

        public async Task<bool> RegisterProvider (string userId, string token, ProviderSelection provider)
        {
            try
            {
                await _repository.RegisterProvider(userId, token, provider);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnregisterProvider (string userId)
        {
            try
            {
                await _repository.UnregisterProvider(userId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsRegistered (string userId)
            => await _repository.UserIsRegistered(userId);

        public async Task<bool> IsValidToken (string token, ProviderSelection provider)
            => await _factory.GetProvider(provider).IsValidToken(token);
    }
}
