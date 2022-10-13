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

        GetMonthReadingsAsync();
        
        
    }


    [RelayCommand]
    async Task AddNewMonthPage()
    {
        await Shell.Current.GoToAsync($"{nameof(HistoryMonthAddPage)}", true, 
            new Dictionary<string, object>
            {
                {"MonthReadings", MonthReadings }
            });

    }

    [RelayCommand]
    async Task DeleteMonth(MonthReading monthReading)
    {
        if (IsBusy || monthReading is null)
            return;
        try
        {
            IsBusy = true;
            
            if (await historyService.DeleteReading(monthReading))
            {
                MonthReadings.Remove(monthReading);
            }

        }
        catch(Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", "Unable to delete", "OK");
        }
        finally
        {
            IsBusy = false;
        }

    }

    [RelayCommand]
    async public void GetMonthReadingsAsync()
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
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

}

