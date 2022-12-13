using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class UsageDataPage : ContentPage
{
	public UsageDataPage(UsageDataViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        ((UsageDataViewModel)BindingContext).RefreshMeterData();
        base.OnNavigatedTo(args);
    }
}