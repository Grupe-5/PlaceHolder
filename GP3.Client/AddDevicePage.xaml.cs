using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class AddDevicePage : ContentPage
{
	public AddDevicePage(AddDeviceViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        Shell.SetTabBarIsVisible(this, false);
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}