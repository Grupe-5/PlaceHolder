using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

	private void PriceChangedHandler(object sender, EventArgs e)
	{
		((SettingsViewModel)(BindingContext)).ChangedNotifSettings(true, false);
	}

	private void LowestPriceHandler(object sender, EventArgs e)
	{
		((SettingsViewModel)(BindingContext)).ChangedNotifSettings(false, true);
	}
}