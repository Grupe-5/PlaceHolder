
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using System.Collections.ObjectModel;

namespace GP3.Client.ViewModels;

[QueryProperty("MeterHistory", "MeterHistory")]
public partial class APITokenPageViewModel : BaseViewModel
{

    private Color redColor = new Color(226, 54, 54);
    private Color greyColor = new Color(246, 246, 246);
    public APITokenPageViewModel()
    {
        tokenFieldBorderColor = greyColor;
        errorText = "";
    }

    [ObservableProperty]
    string apiToken;

    [ObservableProperty]
    Color tokenFieldBorderColor;
    
    [ObservableProperty]
    public ObservableCollection<MeterHistory> meterHistory;

    [ObservableProperty]
    public string errorText;

    [RelayCommand]
    async Task GoBackAsync()
    {
        MeterHistory mh = meterHistory.First();
        mh.monthEstCost = 64;
        await Shell.Current.GoToAsync("../..");

    }

    [RelayCommand]
    void ApiTokenClicked()
    {
        TokenFieldBorderColor = greyColor;
        ErrorText = "";
    }

    [RelayCommand]
    async void AddMeter()
    {
        if(IsBusy)
            return;

        if(string.IsNullOrEmpty(ApiToken))
        {
            ActivateError("Please fill the field!");
            return;
        }
        IsBusy = true;

        // Call things
        meterHistory.First().apiToken = ApiToken;
        await Shell.Current.GoToAsync("../..");
        
        IsBusy = false;
    }
    void ActivateError(string errorText)
    {
        ErrorText = errorText;
        TokenFieldBorderColor = redColor;
    }
}

