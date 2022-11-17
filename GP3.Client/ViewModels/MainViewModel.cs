using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using GP3.Client.Models;

namespace GP3.Client.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly AuthService authService;

        public MainViewModel(AuthService authService)
        {
            this.authService = authService;
            SkipLoginIfValid();
        }

        private async void SkipLoginIfValid()
        {
            await authService.LoadAuth();
            if (await authService.IsSignedIn())
            {
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
        }
        

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;
        
        [RelayCommand]
        async Task LogIn()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)){
                await Shell.Current.DisplayAlert("Error!", "Please fill all fields.", "OK");
                return;
            }
            try
            {
                IsBusy = true;
                await authService.LoginAsync(email, password);
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
            catch (FirebaseAuthException ex)
            {
                await Shell.Current.DisplayAlert("Error!", APIErrorParser.ParseErrorToString(ex), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        [RelayCommand]
        async Task GoToForgotPassword()
        {
            await Shell.Current.DisplayAlert("Error!", "Not implemented!", "OK");
        }
    }
}
