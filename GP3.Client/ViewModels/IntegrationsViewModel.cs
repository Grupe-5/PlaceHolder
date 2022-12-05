using CommunityToolkit.Mvvm.Input;
using DevExpress.Maui.Core.Internal;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Common.Entities;
using System.Collections.ObjectModel;


namespace GP3.Client.ViewModels
{
    public partial class IntegrationsViewModel: BaseViewModel
    {
        private readonly IIntegrationApi _api;
        public ObservableCollection<IntegrationCallback> Integrations { get; } = new();

        public IntegrationsViewModel(IIntegrationApi api)
        {
            Title = "Devices Management";
            _api = api;
            RefreshIntegrationsAsync();
        }

        [RelayCommand]
        async Task EditDevice(IntegrationFormatted currDevice)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            // IntegrationFormatted deviceCopy = currDevice.Clone();

            await Shell.Current.GoToAsync($"{nameof(EditDevicePage)}", true,
                new Dictionary<string, object>
                {
                    {"Devices", Integrations },
                    // {"Device", deviceCopy },
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

        private async Task RefreshIntegrationsAsync()
        {
            try
            {
                var integrationList = await _api.GetIntegrationsAsync();
                IsBusy = true;
                Integrations.Clear();
                Integrations.InsertRange(0, integrationList);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
