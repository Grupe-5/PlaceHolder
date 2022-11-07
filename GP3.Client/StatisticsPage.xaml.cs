using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class StatisticsPage : ContentPage
{
	public StatisticsPage(StatisticsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}