using System.Collections.ObjectModel;
using GP3.Client.Services;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using GP3.Client.Refit;
using System.ComponentModel;

namespace GP3.Client.ViewModels;

[QueryProperty("MeterHistory", "MeterHistory")]
public partial class UsageDataViewModel : BaseViewModel
{
    private readonly IHistoryApi _api;
    public ObservableCollection<MeterHistory> MeterHistory { get; } = new();
    
    public UsageDataViewModel(IHistoryApi api)
    {
        MeterHistory.CollectionChanged += _innerStuff_CollectionChanged;
        _api = api;
        Title = "Energy Meter Data";

        GetMeterData();
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
    }

    async void GetMeterData()
    {
        IsBusy = true;
        try
        {
            if (await _api.ProviderIsRegistered())
            {
                double currDraw = await _api.GetCurrentDraw();
                double dailyUsage = await _api.GetDailyUsage();
                double monthlyusage = await _api.GetMonthlyUsage();

                MeterHistory.Add(new MeterHistory(currDraw, dailyUsage, dailyUsage * 0.43918, monthlyusage, monthlyusage * 0.43918));
            }

        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error!", e.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void _innerStuff_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Object item in e.NewItems)
            {
                ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
            }
        }
        if (e.OldItems != null)
        {
            foreach (Object item in e.OldItems)
            {
                ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
            }
        }
    }

    private async void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        isMeterRegistred = true;
    }

}

