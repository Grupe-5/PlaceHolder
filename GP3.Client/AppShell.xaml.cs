namespace GP3.Client;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(StatisticsPage), typeof(StatisticsPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
		Routing.RegisterRoute(nameof(ChooseProviderPage), typeof(ChooseProviderPage));	
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
		Routing.RegisterRoute(nameof(EditDevicePage), typeof(EditDevicePage));
        Routing.RegisterRoute(nameof(AddDevicePage), typeof(AddDevicePage));
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(AddAPITokenPage), typeof(AddAPITokenPage));
        Routing.RegisterRoute(nameof(ConnectDevice), typeof(ConnectDevice));
    }
}
