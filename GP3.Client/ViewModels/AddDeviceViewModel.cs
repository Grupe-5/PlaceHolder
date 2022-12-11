using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Common.Entities;
using System.Collections.ObjectModel;

namespace GP3.Client.ViewModels;

[QueryProperty("Devices", "Devices")]
public partial class AddDeviceViewModel: BaseViewModel
{
    IIntegrationApi _api;
    public AddDeviceViewModel(IIntegrationApi api)
    {
        Title = "Add Device";
        reasonNames = Enum.GetNames(typeof(IntegrationCallbackReason));
        _api = api;
    }

    [ObservableProperty]
    string[] reasonNames;

    [ObservableProperty]
    string callbackUrl;

    [ObservableProperty]
    IntegrationCallbackReason callbackReason;

    [ObservableProperty]
    public ObservableCollection<IntegrationFormatted> devices;

    [RelayCommand]
    public async void AddNewDevice()
    {
        /* POST to server */
        IntegrationCallback clbk = null;
        try
        {
            clbk = await _api.AddIntegrationAsync(CallbackUrl, callbackReason);
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error!", "Failed to save callback!", "OK");
        }
        finally
        {
            if (clbk != null)
            {
                devices.Add(new IntegrationFormatted(clbk));
            }
            await GoBackAsync();
        }
    }
    async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}





