using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Refit;
using System.Collections.ObjectModel;


namespace GP3.Client.ViewModels
{
    public partial class IntegrationsViewModel: BaseViewModel
    {
        private readonly IIntegrationApi _api;
        public ObservableCollection<IntegrationFormatted> Integrations { get; } = new();

        [ObservableProperty]
        bool isRefreshing;

        public IntegrationsViewModel(IIntegrationApi api)
        {
            Title = "Devices Management";
            _api = api;
        }

        [RelayCommand]
        async Task EditDevice(IntegrationFormatted currDevice)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            IntegrationFormatted deviceCopy = (IntegrationFormatted)currDevice.Clone();

            await Shell.Current.GoToAsync($"{nameof(EditDevicePage)}", true,
                new Dictionary<string, object>
                {
                    {"Devices", Integrations },
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
                    {"Devices", Integrations },
                });
            IsBusy = false;
        }

        public async Task RefreshIntegrationsAsync()
        {
            try
            {
                IsRefreshing = true;
                var integrationList = await _api.GetIntegrationsAsync();
                Integrations.Clear();
                foreach (var x in integrationList)
                {
                    Integrations.Add(new IntegrationFormatted(x));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
