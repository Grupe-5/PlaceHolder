
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using System.Collections.ObjectModel;

namespace GP3.Client.ViewModels;

[QueryProperty("Devices", "Devices")]
public partial class ConnectDeviceViewModel : BaseViewModel
{

    public ConnectDeviceViewModel()
    {
        IsBusy = true;
        Thread.Sleep(2500);
        IsBusy = false;
    }

    [ObservableProperty]
    public ObservableCollection<DeviceIntegration> devices;


    [RelayCommand]
    public async Task ConnectDevice()
    {
        DeviceIntegration CurrDevice = new DeviceIntegration(420, "LG DishWasher", "Washer ", new TimeSpan(), new TimeSpan(), false, 0, false, "dishwasher.png"); 
        devices.Add(CurrDevice);

        await Shell.Current.GoToAsync("../..");
    }

}


