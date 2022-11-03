using GP3.Client.ViewModels;

namespace GP3.Client;

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

}

