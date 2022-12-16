using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using System.Collections.ObjectModel;


namespace GP3.Client.ViewModels
{
    public partial class IntegrationsViewModel : BaseViewModel
    {
        public ObservableCollection<DeviceIntegration> DevicesIntegrations { get; } = new();

        public IntegrationsViewModel()
        {
            Title = "Integrations page";
            DeviceIntegration integrations;

            /* Call API */
            /* Only for testing */
            integrations = new(1, "Smart Toaster", "Toaster", new TimeSpan(6, 5, 0), new TimeSpan(7, 5, 0), true, 20, false, "toaster.svg");    
            DevicesIntegrations.Add(integrations);
            integrations = new(1, "Smart Lamp ", "Lamp", new TimeSpan(7, 5, 0), new TimeSpan(8, 5, 0), false, 10, true, "lamp.png");
            DevicesIntegrations.Add(integrations);

        }

        [RelayCommand]
        async Task EditDevice(DeviceIntegration currDevice)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            DeviceIntegration deviceCopy = currDevice.Clone();

            await Shell.Current.GoToAsync($"{nameof(EditDevicePage)}", true,
                new Dictionary<string, object>
                {
                    {"Devices", DevicesIntegrations },
                    {"Device", deviceCopy },
                });
            IsBusy = false;
        }

        [RelayCommand]
        async Task AddNewDevice()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(AddDevicePage)}", true,
                new Dictionary<string, object>
                {
                    {"Devices", DevicesIntegrations },
                });
            IsBusy = false;
        }
    }
}