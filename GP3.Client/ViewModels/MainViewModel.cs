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

        private Color redColor = new Color(226, 54, 54);
        private Color greyColor = new Color(246, 246, 246);

        public MainViewModel(AuthService authService)
        {
            this.authService = authService;
            SkipLoginIfValid();

            emailFieldBorderColor = greyColor;
            pswFieldBorderColor = greyColor;

            email = "";
            HideError();
        }

        private async void SkipLoginIfValid()
        {
            await authService.LoadAuth();
            if (authService.IsSignedIn())
            {
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
        }

        [ObservableProperty]
        Color emailFieldBorderColor;

        [ObservableProperty]
        Color pswFieldBorderColor;

        [ObservableProperty]
        string errorText;

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
                ActivateError("Please fill all the fields!");
                return;
            }

            try
            {
                IsBusy = true;
                await authService.LoginAsync(email, password);
                HideError();
                await Shell.Current.GoToAsync(nameof(HomePage));

            }
            catch (FirebaseAuthException ex)
            {
                ActivateError(AuthService.ParseErrorToString(ex));
                Password = "";
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
        void EmailClicked()
        {
            EmailFieldBorderColor = greyColor;
            CheckIfRemoveErroMsg();
        }       
        
        [RelayCommand]
        void PswClicked()
        {
            PswFieldBorderColor = greyColor;
            CheckIfRemoveErroMsg();
        }

        void CheckIfRemoveErroMsg()
        {
            if(PswFieldBorderColor.Equals(greyColor) && EmailFieldBorderColor.Equals(greyColor))
            {
                ErrorText = "";
            }
        }

        void ActivateError(string errorText)
        {
            ErrorText = errorText;
            if (string.IsNullOrEmpty(email))
                EmailFieldBorderColor = redColor;
            if (string.IsNullOrEmpty(password))
                PswFieldBorderColor = redColor;
        }
        void HideError()
        {
            password = "";
            PswFieldBorderColor = greyColor;
            EmailFieldBorderColor = greyColor;
        }

    }
}
