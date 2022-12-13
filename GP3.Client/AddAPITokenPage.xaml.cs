using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class AddAPITokenPage : ContentPage
{
	public AddAPITokenPage(APITokenPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        Shell.SetTabBarIsVisible(this, false);
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}