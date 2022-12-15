using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

	private async void PriceChangedHandler(object sender, EventArgs e)
	{
		await ((SettingsViewModel)(BindingContext)).ChangedNotifSettings(true, false);
	}

	private async void LowestPriceHandler(object sender, EventArgs e)
	{
		await ((SettingsViewModel)(BindingContext)).ChangedNotifSettings(false, true);
	}

	private async void PrivacyAlert(object sender, EventArgs e)
	{
		await Shell.Current.DisplayAlert(
			"Our Privacy Policy",
            "We take the utmost care to ensure that GDPR laws are followed.",
			"I understand");
	}
}