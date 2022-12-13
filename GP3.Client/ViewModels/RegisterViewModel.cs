using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;

namespace GP3.Client.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        readonly AuthService authService;
        private Color redColor = new Color(226, 54, 54);
        private Color greyColor = new Color(246, 246, 246);
        public RegisterViewModel(AuthService authService)
        {
            this.authService = authService;
            emailFieldBorderColor = greyColor;
            pswFieldBorderColor = greyColor;
            pswRepeatFieldBorderColor = greyColor;
        }

        [ObservableProperty]
        Color emailFieldBorderColor;

        [ObservableProperty]
        Color pswFieldBorderColor;

        [ObservableProperty]
        Color pswRepeatFieldBorderColor;

        [ObservableProperty]
        string emailErrorText;

        [ObservableProperty]
        string pswErrorText;

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

            if (CheckIfEmptyFields())
                return;

            if (!IsValidEmail())
            {
                ActivateEmailError("Invalid Email!");
                return;
            }

            if (!CheckIfValidPassword())
            {
                return;
            }

            try
            {
                IsBusy = true;
                await authService.RegisterAsync(email, password);
                HidePswError(true, true);
                HideEmailError();
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
            catch (Exception ex)
            {
                ActivatePswError("Failed to register", true, true);
                ActivateEmailError("");
            }
            finally
            {
                IsBusy = false;
            }
        }

        bool IsValidEmail()
        {
            return Regex.IsMatch(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        }

        [RelayCommand]
        void EmailClicked()
        {
            EmailFieldBorderColor = greyColor;
            EmailErrorText = "";
        }

        [RelayCommand]
        void PswClicked()
        {
            PswFieldBorderColor = greyColor;
            CheckIfRemovePswErroMsg();
        }     
        
        [RelayCommand]
        void PswRepeatClicked()
        {
            PswRepeatFieldBorderColor = greyColor;
            CheckIfRemovePswErroMsg();
        }

        bool CheckIfValidPassword()
        {
            if (password != repeatedPassword)
            {
                ActivatePswError("Passwords do not match!", true, true);
                return false;
            }

            if (password.Length < 8)
            {
                ActivatePswError("Password must be at least 8 characters long!", true, true);
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                ActivatePswError("Password must contain at least 1 upppercase letter!", true, true);
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                ActivatePswError("Password must contain at least 1 number!", true, true);
                return false;
            }
            if (!password.Any(char.IsSymbol))
            {
                ActivatePswError("Password must contain at least 1 special symbol!", true, true);
                return false;
            }

            return true;
        }
        void ActivateEmailError(string errorText)
        {
            EmailErrorText = errorText;
            EmailFieldBorderColor = redColor;
        }

        void HideEmailError()
        {
            EmailErrorText = "";
            EmailFieldBorderColor = greyColor;
        }

        void ActivatePswError(string errorText, bool turnPswRed, bool turnPswRepRed)
        {
            PswErrorText = errorText;
            if (turnPswRed)
                PswFieldBorderColor = redColor;
            if (turnPswRepRed)
                PswRepeatFieldBorderColor = redColor;

            CheckIfRemovePswErroMsg();
        }

        void HidePswError(bool turnPswGrey, bool turnPswRepGrey)
        {
            if (turnPswGrey)
                PswFieldBorderColor = greyColor;
            if (turnPswRepGrey)
                PswRepeatFieldBorderColor = greyColor;
            PswErrorText = "";
        }    

        bool CheckIfEmptyFields()
        {
            bool isEmailEmpty = string.IsNullOrEmpty(email);
            bool isPswEmpty = string.IsNullOrEmpty(password);
            bool isRepPswEmpty = string.IsNullOrEmpty(repeatedPassword);

            if (isEmailEmpty || isPswEmpty || isRepPswEmpty)
            {
                if (isEmailEmpty)
                    ActivateEmailError("Enter an email!");

                if (isPswEmpty || isRepPswEmpty)
                    ActivatePswError("Enter both password fields!", isPswEmpty, isRepPswEmpty);
                
                return true;
            }
            return false;
        }

        void CheckIfRemovePswErroMsg()
        {
            if (PswFieldBorderColor.Equals(greyColor) && PswRepeatFieldBorderColor.Equals(greyColor))
                PswErrorText = "";
        }

    }
}
