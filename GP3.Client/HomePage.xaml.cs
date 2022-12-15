using GP3.Client.ViewModels;
using Plugin.Firebase.CloudMessaging;

namespace GP3.Client;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        SubscribeToNotifications();
    }

    private async static void SubscribeToNotifications()
    {
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        await CrossFirebaseCloudMessaging.Current.SubscribeToTopicAsync("ha");
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        ((HomeViewModel)BindingContext).SetLowPriceLine();
    }
}