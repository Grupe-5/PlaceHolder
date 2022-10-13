using Client.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Client.Services;
using CommunityToolkit.Mvvm.Input;


namespace Client.ViewModels;
public partial class HistoryViewModel : BaseViewModel
{
    HistoryService historyService;
    public ObservableCollection<MonthReading> MonthReadings { get; } = new();

    public HistoryViewModel(HistoryService historyService)
    {
        this.historyService = historyService;
        Title = "HistoryPage";
        // Make this more pretty
        try
        {
            GetMonthReadingsAsync();
        }
        catch(Exception ex)
        {

        }
    }
  



    [RelayCommand]
    async Task GotToMonthAsync(MonthReading monthReading)
    {
        if (monthReading is null)
            return;


        await Shell.Current.GoToAsync($"{nameof(HistoryMonthPage)}",true,new Dictionary<string, object>
        {
            {"MonthReading ", monthReading }
        });
    }

    [RelayCommand]
    async Task GetMonthReadingsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var readings = await historyService.GetReadings();

            if (MonthReadings.Count != 0)
                MonthReadings.Clear();

            foreach (var reading in readings)
                MonthReadings.Add(reading);

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!","Unable to get data.","OK");
        }
        finally
        {
            IsBusy = false;
            
        }
    }
}

