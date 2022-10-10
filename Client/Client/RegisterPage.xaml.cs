using Client.ViewModels;
using System.Text.RegularExpressions;

namespace Client;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    bool isValidEmailRegex(string email)
    {
        /*bool isEmail = Regex.IsMatch(email,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase);*/

        bool isEmail = Regex.IsMatch(email,
            @"^([\w]+)@([\w]+)((\.(\w){2,3})+)$",
            RegexOptions.IgnoreCase);

        return isEmail;

    }

    private async void CheckRegex(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Username.Text) && string.IsNullOrEmpty(Password.Text) && string.IsNullOrEmpty(RepeatedPassword.Text))
        {
            await DisplayAlert("Error", "Do not leave empty fields!", "OK");
            return;
        }

        if (isValidEmailRegex(Username.Text) == false)
        {
           
            await DisplayAlert("Error", "Your email adress is incorrect!", "OK");
            Username.Text = string.Empty;
            Password.Text = string.Empty;
            RepeatedPassword.Text = string.Empty;

        } else
        {
            await DisplayAlert("Success!", "You have been registered!", "OK");
            Username.Text = string.Empty;
            Password.Text = string.Empty;
            RepeatedPassword.Text = string.Empty;
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
   
    }
}