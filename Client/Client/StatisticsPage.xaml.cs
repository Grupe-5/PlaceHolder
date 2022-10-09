using Client.ViewModels;

namespace Client;

public partial class StatisticsPage : ContentPage
{
	public StatisticsPage(StatisticsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}