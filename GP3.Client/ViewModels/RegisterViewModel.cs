using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using Firebase.Auth;

namespace GP3.Client.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        readonly AuthService authService;

        public RegisterViewModel(AuthService authService)
        {
            this.authService = authService;
        }

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await Shell.Current.DisplayAlert("Error!", "Please fill all fields.", "OK");
                return;
            }
            try
            {
                IsBusy = true;
                await authService.RegisterAsync(email, password);
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
    }
}
