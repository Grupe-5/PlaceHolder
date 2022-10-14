namespace Client;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(StatisticsPage), typeof(StatisticsPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
		Routing.RegisterRoute(nameof(HistoryMonthAddPage), typeof(HistoryMonthAddPage));	
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

    }
}
