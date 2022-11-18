using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using System.Collections.ObjectModel;


namespace GP3.Client.ViewModels
{
    public partial class IntegrationsViewModel: BaseViewModel
    {
        public ObservableCollection<DeviceIntegration> DevicesIntegrations { get; } = new();

        public IntegrationsViewModel()
        {
            Title = "Integrations page";
            DeviceIntegration integrations;

            /* Call API */
            /* Only for testing */
            bool isRunning = false;
            for (int i = 0; i < 5; i++)
            {
                integrations = new(69*i, "Noice " + i.ToString(), "Washer " + i.ToString(), new TimeSpan(i, i, i), new TimeSpan(i, i, i), isRunning, i*i, isRunning);
                isRunning = !isRunning;
                DevicesIntegrations.Add(integrations);
            }
            /* TESTING */
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
