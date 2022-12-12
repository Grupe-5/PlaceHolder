using System.Collections.ObjectModel;
using GP3.Client.Services;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GP3.Client.ViewModels;

[QueryProperty("MeterHistory", "MeterHistory")]
public partial class UsageDataViewModel : BaseViewModel
{

    UsageDataService _historyService;
    public ObservableCollection<MeterHistory> MeterHistory { get; } = new();

    public UsageDataViewModel(UsageDataService historyService)
    {
        _historyService = historyService;
        Title = "Energy Meter Data";

        MeterHistory.Add(new MeterHistory(3, 3, 3, 3, 3));



    }
    async Task nice()
    {
        await Shell.Current.DisplayAlert("Error!", "here", "OK");
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
        //await Shell.Current.GoToAsync($"{nameof(HistoryMonthAddPage)}");
        await Shell.Current.GoToAsync($"{nameof(ChooseProviderPage)}", true, 
            new Dictionary<string, object>
            {
                {"MeterHistory", MeterHistory}
            });
        IsMeterRegistred = !IsMeterRegistred;
    }

}

