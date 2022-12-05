using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.ViewModels;

[QueryProperty("Devices", "Devices")]
public partial class AddDeviceViewModel: BaseViewModel
{
    public AddDeviceViewModel()
    {
        Title = "Add Device";
    }

    [ObservableProperty]
    public IntegrationFormatted device = new();

    [ObservableProperty]
    public ObservableCollection<IntegrationFormatted> devices;

    [RelayCommand]
    public async void AddNewDevice()
    {

        /* TODO ADD Validation */
        /* TODO Call API */
        /* TODO: Clone here */
        IntegrationFormatted CurrDevice = device;
        devices.Add(CurrDevice);

        await GoBackAsync();
    }
    async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}





