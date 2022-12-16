using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class ConnectDevice : ContentPage
{
	public ConnectDevice(ConnectDeviceViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        Shell.SetTabBarIsVisible(this, false);
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
 
}