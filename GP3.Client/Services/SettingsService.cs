
using Newtonsoft.Json;

namespace GP3.Client.Services
{
    public class SettingsService
    {
        private string fullPath;
        public SettingsService()
        {
            string path = FileSystem.Current.CacheDirectory;
            fullPath = Path.Combine(path, "UserSetting.txt");
        }
        public UserSettings GetSettings1()
        {
           
            string userSettingsJson = File.ReadAllText(fullPath);
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userSettingsJson);
            
            return userSettings;
        }

        public async Task<bool> PutSettings(UserSettings userSettings)
        {
            string userSettingsJson = JsonConvert.SerializeObject(userSettings);

            try
            {
                File.WriteAllText(fullPath, userSettingsJson);
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
                return false;
            }

            return true;
        }
    }
}
