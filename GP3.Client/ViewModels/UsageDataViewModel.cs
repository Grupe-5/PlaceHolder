using System.Collections.ObjectModel;
using GP3.Client.Services;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using GP3.Client.Refit;
using System.Threading.Tasks;

namespace GP3.Client.ViewModels;

[QueryProperty("MeterHistory", "MeterHistory")]
public partial class UsageDataViewModel : BaseViewModel
{
    private readonly IHistoryApi _api;
    public ObservableCollection<MeterHistory> MeterHistory { get; } = new();

    public UsageDataViewModel(IHistoryApi api)
    {
        _api = api;
        Title = "Energy Meter Data";

        

        MeterHistory.Add(new MeterHistory(3, 3, 3, 3, 3));
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotMeterRegistred))]
    bool isMeterRegistred;

    public bool IsNotMeterRegistred => !isMeterRegistred;

    [ObservableProperty]
    double currentDraw;


    [RelayCommand]
    async Task AddNewMonthPage()
    {
        await Shell.Current.GoToAsync($"{nameof(ChooseProviderPage)}", true, 
            new Dictionary<string, object>
            {
                {"MeterHistory", MeterHistory}
            });
        IsMeterRegistred = !IsMeterRegistred;
    }

    async Task GetApiStuff()
    {
        bool a = await _api.ProviderIsRegistered();
        await Shell.Current.DisplayAlert("Error!", a.ToString(), "OK");
    }

}

