using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class ChooseProviderPage : ContentPage
{
	public ChooseProviderPage(ChooseProviderViewModel viewModel)
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