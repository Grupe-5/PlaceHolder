using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Common.Entities;
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
        public IIntegrationApi _api;
        public EditDeviceViewModel(IIntegrationApi api)
        {
            Title = "Update Device";
            reasonNames = Enum.GetNames(typeof(IntegrationCallbackReason));
            _api = api;
        }

        [ObservableProperty]
        string[] reasonNames;

        [ObservableProperty]
        public IntegrationFormatted device;

        [ObservableProperty]
        public ObservableCollection<IntegrationFormatted> devices;

        [RelayCommand]
        public async void UpdateDeviceInformation()
        {
            if (IsBusy || devices is null)
                return;

            try
            {
                IsBusy = true;
                IntegrationFormatted currDevice = getCurrDevice();
                if (currDevice is null)
                    await Shell.Current.DisplayAlert("Error!", "Something went horribly wrong!", "OK");
                else
                {
                    await _api.RemoveIntegrationAsync(currDevice);
                    await _api.AddIntegrationAsync(Device);
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error!", "Failed to update callback!", "OK");
            }
            finally
            {
                IsBusy = false;
                await GoBackAsync();
            }
        }

        [RelayCommand]
        public async void DeleteDevice()
        {
            var curr = getCurrDevice();
            await _api.RemoveIntegrationAsync(curr);
            await GoBackAsync();
        }
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private IntegrationFormatted getCurrDevice()
        {
            return devices.Where(x => x.Id == Device.Id).First();
        }
    }
}
