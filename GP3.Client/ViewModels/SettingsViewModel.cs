using Client.Model;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Client.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        SettingsService settingsService;


        [ObservableProperty]
        UserSettings userSettings;

        public SettingsViewModel(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            userSettings = new();
            Title = "Settings";
            
            GetSettingsAsync();
        }

        [RelayCommand]
        async public void SaveSettings()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await settingsService.PutSettings(userSettings);
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

                userSettings = await settingsService.GetSettings1();
                
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
