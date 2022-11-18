using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        Shell.SetTabBarIsVisible(this, false);
	}
}