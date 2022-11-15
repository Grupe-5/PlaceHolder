using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.ViewModels
{
    public partial class IntegrationsViewModel: BaseViewModel
    {
        public ObservableCollection<DeviceIntegration> DevicesIntegrations { get; } = new();

        public IntegrationsViewModel()
        {
            Title = "Integrations page";
            DeviceIntegration integrations;

            bool isRunning = false;

            for (int i = 0; i < 5; i++)
            {
                integrations = new(69*i, "Noice " + i.ToString(), "Washer " + i.ToString(), new TimeSpan(i, i, i), new TimeSpan(i, i, i), isRunning);

                isRunning = !isRunning;
                DevicesIntegrations.Add(integrations);
            }


        }

        [RelayCommand]
        async Task EditDevice(DeviceIntegration currDevice)
        {
            DeviceIntegration deviceCopy = currDevice.Clone();

            await Shell.Current.GoToAsync($"{nameof(EditDevicePage)}", true,
                new Dictionary<string, object>
                {
                    {"Devices", DevicesIntegrations },
                    {"Device", deviceCopy },
                });                
        }

        [RelayCommand]
        async Task AddNewDevice()
        {

            //await Shell.Current.GoToAsync($"{nameof(EditDevicePage)}", true,
            //    new Dictionary<string, object>
            //    {
            //        {"Devices", DevicesIntegrations },
            //        {"DeviceName", null },
            //        {"IsUpdatePage", false }
            //    });
        }
    }
}
