using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Refit;

namespace GP3.Client.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private readonly AuthService _authService;
        private readonly IHistoryApi _historyApi;

        public SettingsViewModel(SettingsService settingsService, AuthService authService, IHistoryApi historyApi)
        {
            _settingsService = settingsService;
            _authService = authService;
            _historyApi = historyApi;

            Title = "Settings";
            GetSettingsAsync();
        }

        [ObservableProperty]
        UserSettings userSettings;

        [RelayCommand]
        async Task SignOut()
        {
            _authService.NullAuth();
            await _historyApi.UnregisterProvider();
            await Shell.Current.GoToAsync("///" + nameof(MainPage));
        }

        [RelayCommand]
        async public void SaveSettings()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                _settingsService.Settings = userSettings;
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

        [RelayCommand]
        public static async void OpenMailApp()
        {
            if (Email.Default.IsComposeSupported)
            {
                string[] recipients = new[] { "placeholder@gp3.com"};

                var message = new EmailMessage
                {
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };

                await Email.Default.ComposeAsync(message);
            }
        }
        
        async public void GetSettingsAsync()
        {
            try
            {
                IsBusy = true;

                userSettings = _settingsService.Settings;
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
