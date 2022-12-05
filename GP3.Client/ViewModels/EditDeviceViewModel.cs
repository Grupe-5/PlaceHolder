using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.ViewModels
{
    [QueryProperty("Devices", "Devices")]
    [QueryProperty("Device", "Device")]
    public partial class EditDeviceViewModel: BaseViewModel
    {

        public EditDeviceViewModel()
        {
            Title = "Update Device";
        }

        [ObservableProperty]
        public IntegrationFormatted device;

        [ObservableProperty]
        public ObservableCollection<IntegrationFormatted> devices;

        [RelayCommand]
        public async void UpdateDeviceInformation()
        {
            if (IsBusy || devices is null)
                return;

            IntegrationFormatted currDevice = getCurrDevice();
            if (currDevice is null)
                await Shell.Current.DisplayAlert("Error!", "Something went horribly wrong!", "OK");

            /* TODO: Do clone here */
            currDevice = device;

            await GoBackAsync();
        }

        [RelayCommand]
        public async void DeleteDevice()
        {
            devices.Remove(getCurrDevice());

            await GoBackAsync();
        }
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private IntegrationFormatted getCurrDevice()
        {
            return devices.Where(x => x.Id == device.Id).First();
        }

    }
}
