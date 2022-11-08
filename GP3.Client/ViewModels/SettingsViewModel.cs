using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace GP3.Client.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
            userSettings = new();
            Title = "Settings";
            
            GetSettingsAsync();
        }

        [ObservableProperty]
        UserSettings userSettings;

        [RelayCommand]
        async public void SaveSettings()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await _settingsService.PutSettings(userSettings);
                await Shell.Current.DisplayAlert("","Settings saved successfully!","Ok");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        async public void GetSettingsAsync()
        {
            try
            {
                IsBusy = true;

                userSettings = await _settingsService.GetSettings1();
                
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
