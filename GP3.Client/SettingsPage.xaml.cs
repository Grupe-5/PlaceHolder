using Client.ViewModels;

namespace Client;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
        InitializeComponent();
		BindingContext = vm;
    }
}