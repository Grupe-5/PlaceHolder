using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
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

        [ObservableProperty]
        string repeatedPassword;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (IsBusy)
                return;

            if(password != repeatedPassword)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "Passwords do not match",
                    "OK");

                return;
            }

            if (password.Length < 8)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "Password must be at least 8 characters long!",
                    "OK");

                return;
            }

            if (IsValidPasswordRegex(password) == false)
            {
                await Shell.Current.DisplayAlert(
                    "Error", 
                    "Password must contain at least 1 upppercase, " +
                    "lowercase characters and special symbols!\n",
                    "OK");
               
                return;
            }

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
                await Shell.Current.DisplayAlert("Error!", AuthService.ParseErrorToString(ex), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        bool IsValidPasswordRegex(string password)
        {
            bool isPassword = Regex.IsMatch(
                password,
                @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])(?=^[^"",']+$).{8,}$");
            return isPassword;
        }
    }
}
