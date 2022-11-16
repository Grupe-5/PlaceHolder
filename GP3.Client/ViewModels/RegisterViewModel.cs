using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;

namespace GP3.Client.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        readonly AuthService authService;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string repeatedPassword;

        public RegisterViewModel(AuthService authService)
        {
            this.authService = authService;
        }

        bool IsValidPasswordRegex(string password)
        {
            bool isPassword = Regex.IsMatch(
                password,
                @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])(?=^[^"",']+$).{8,}$");
            return isPassword;
        }


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

            try
            {
                IsBusy = true;
                await authService.RegisterAsync(email, password);
                await Shell.Current.GoToAsync(nameof(HomePage));
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
