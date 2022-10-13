using Client.ViewModels;
using System.Text.RegularExpressions;

namespace Client;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
        Shell.SetTabBarIsVisible(this, false);
        BindingContext = vm;
	}

    private void ClearEntryField()
    {
        Username.Text = string.Empty;
        Password.Text = string.Empty;
        RepeatedPassword.Text = string.Empty;
    }

    bool IsValidEmailRegex(string email)
    {
        bool isEmail = Regex.IsMatch(email,
                                     @"^([\w]+\.*[\w]+)@([\w]+)((\.(\w){2,3})+)$",
                                     RegexOptions.IgnoreCase);
        return isEmail;
    }

    bool IsValidPasswordRegex(string password)
    {
        //password must be at least 8 char long and
        //must contain 1 or more digits, uppercase and lowecase letters, special symbols
        bool isPassword = Regex.IsMatch(password, 
                                        @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%/^&*-]).{8,}$");
        return isPassword;
    }

    private async void CheckRegex(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Username.Text) && string.IsNullOrEmpty(Password.Text) && string.IsNullOrEmpty(RepeatedPassword.Text))
        {
            await DisplayAlert("Error", "Do not leave empty fields!", "OK");
            return;
        }

        if (Password.Text != RepeatedPassword.Text)
        {
            await DisplayAlert("Error", "Passwords do not match", "OK");
            ClearEntryField();
            return;
        }

        if (IsValidEmailRegex(Username.Text) == false)
        {
            await DisplayAlert("Error", "Your email adress is incorrect!", "OK");
            ClearEntryField();
            return;
        }

        if(Password.Text.Length < 8)
        {
            await DisplayAlert("Error", "Password must be at least 8 characters long!", "OK");
            ClearEntryField();
            return;
        }

        if(IsValidPasswordRegex(RepeatedPassword.Text) == false)
        {
            await DisplayAlert("Error", "Password must contain at least 1 upppercase, lowercase characters and special symbols!", "OK");
            ClearEntryField();
            return;
        }

        await DisplayAlert("Success!", "You have been registered!", "OK");
        ClearEntryField();
        await Shell.Current.GoToAsync(nameof(HomePage));
    }
}