
using MonkeyCache;
using MonkeyCache.FileStore;
using Newtonsoft.Json;

namespace GP3.Client.Services
{
    public class SettingsService
    {
        private const string barrelPrefix = "UserSettings";
        private readonly TimeSpan barrelDuration = TimeSpan.MaxValue;
        private readonly IBarrel _barrel;

        public SettingsService(IBarrel barrel)
        {
            _barrel = barrel;
            if(!_barrel.Exists(barrelPrefix))
            {
                UserSettings userSettings = new UserSettings();
                _barrel.Add(key: barrelPrefix, data: userSettings, expireIn: barrelDuration);
            }
        }

        public UserSettings GetSettings1()
        {
            if (_barrel.Exists(barrelPrefix))
            {
                return _barrel.Get<UserSettings>(barrelPrefix);
            }
            return null;
        }

        public async Task<bool> PutSettings(UserSettings userSettings)
        {
   
            if (_barrel.Exists(barrelPrefix))
            {
                _barrel.Add(key: barrelPrefix, data: userSettings, expireIn: barrelDuration);
                return true;
            }

            return false;
        }
    }
}
