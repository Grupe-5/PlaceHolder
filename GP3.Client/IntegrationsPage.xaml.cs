using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class IntegrationsPage : ContentPage
{
	public IntegrationsPage(IntegrationsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
    

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await ((IntegrationsViewModel)BindingContext).RefreshIntegrationsAsync();
        base.OnNavigatedTo(args);
    }

}