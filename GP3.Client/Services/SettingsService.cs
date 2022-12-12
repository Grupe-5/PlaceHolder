
using GP3.Client.Models;
using MonkeyCache;

namespace GP3.Client.Services
{
    public class SettingsService
    {
        private const string barrelPrefix = "UserSettings";
        private readonly TimeSpan barrelDuration = TimeSpan.MaxValue;
        private readonly IBarrel _barrel;

        public UserSettings Settings { get => _barrel.Get<UserSettings>(barrelPrefix); set => _barrel.Add(key: barrelPrefix, data: value, expireIn: barrelDuration); }
        public SettingsService(IBarrel barrel)
        {
            _barrel = barrel;
            if(!_barrel.Exists(barrelPrefix))
            {
                Settings = new UserSettings();
            }
        }
    }
}
