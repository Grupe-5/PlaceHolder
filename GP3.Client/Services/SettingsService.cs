namespace GP3.Client.Services
{
    public class SettingsService
    {
        UserSettings userSettings;
        public async Task<UserSettings> GetSettings1()
        {            
            userSettings = new UserSettings();
            userSettings.priceChangeNotf = true;
            userSettings.lowestPriceNotf = true;
            userSettings.locations = new string[1];
            userSettings.locations[0] = "Lithuania";
            userSettings.userLocation = "Lithuania";

            // GET API
            return userSettings;
        }

        public async Task<bool> PutSettings(UserSettings userSettings)
        {
            // PUT API   
            return true;
        }
    }
}
