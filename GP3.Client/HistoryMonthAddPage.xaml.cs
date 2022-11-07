using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class HistoryMonthAddPage : ContentPage
{
	public HistoryMonthAddPage(HistoryMonthAddViewModel viewModel)
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