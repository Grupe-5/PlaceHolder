

using Client.Model;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Client.ViewModels
{
    [QueryProperty("MonthReadings", "MonthReadings")]
    public partial class HistoryMonthAddViewModel : BaseViewModel
    {


        public HistoryMonthAddViewModel()
        {
            monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;
        }

        [ObservableProperty]
        string[] monthNames;

        [ObservableProperty]
        string selectedMonth;

        [ObservableProperty]
        string kwhUsed;

        [ObservableProperty]
        string pricePayed;

        [ObservableProperty]
        ObservableCollection<MonthReading> monthReadings;

        [RelayCommand]
        async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task OnAdd()
        {
            // ADD VALIDATION
            MonthReading monthReadingItem = new MonthReading(selectedMonth, Double.Parse(pricePayed), int.Parse(kwhUsed));
            
            HistoryService historyService = new HistoryService();
            await historyService.AddMonth(monthReadingItem);
            monthReadings.Add(monthReadingItem);

            await GoBackAsync();
        }
    }
}
