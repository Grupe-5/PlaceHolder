using GP3.Client.ViewModels;
using Plugin.Firebase.CloudMessaging;

namespace GP3.Client;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        ((HomeViewModel)BindingContext).SetLowPriceLine();
    }
}