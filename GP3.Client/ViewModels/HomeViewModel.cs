using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Client.Services;
using System.Collections.ObjectModel;

namespace GP3.Client.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPriceApi _priceApi;
        private readonly SettingsService _settingsService;
        public ObservableCollection<HourPriceFormated> HourPricesFormated { get; } = new();

        private DateTime today;

        public HomeViewModel(IPriceApi priceApi, SettingsService settingsService)
        {
            _priceApi = priceApi;
            _settingsService = settingsService;
            Title = "Home";

            today = DateTime.Now.Date;
            isToday = true;
            selectedDate = today;
            selectedDateFormated = selectedDate.ToShortDateString();

            currHour = DateTime.Now.Hour + (DateTime.Now.Minute / 60);
            
            GetPricesAsync();

            currUserLowPriceDefinition = _settingsService.Settings.lowPriceMark;
        }

        [ObservableProperty]
        public double averagePrice;

        [ObservableProperty]
        public double currUserLowPriceDefinition;

        [ObservableProperty]
        public double currHour;

        [ObservableProperty]
        public double currPrice;

        [ObservableProperty]
        public string selectedDateFormated;

        [ObservableProperty]
        public DateTime selectedDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotToday))]
        public bool isToday;
        public bool IsNotToday => !isToday;

        [RelayCommand]
        void DecreaseDate()
        {
            IsToday = false;
            selectedDate = SelectedDate.AddDays(-1);
            SelectedDateFormated = selectedDate.ToShortDateString();
            GetPricesAsync();
        }

        [RelayCommand]
        void IncreaseDate()
        {
            if (SelectedDate != today)
            {
                SelectedDate = SelectedDate.AddDays(1);
                IsToday = SelectedDate == today;
                SelectedDateFormated = SelectedDate.ToShortDateString();
                GetPricesAsync();
            }
            else
            {
                IsToday = true;
            }
        }

        async public void GetPricesAsync()
        {
            try
            {
                IsBusy = true;
                var prices = await _priceApi.GetPriceOffsetAsync(SelectedDate);
                int index = 0;
                bool firstSkipped = false;
                HourPriceFormated hourPriceFormated;
                HourPricesFormated.Clear();
                AveragePrice = 0;

                foreach (var price in prices.HourlyPrices)
                {
                    if (firstSkipped)
                    {
                        if(DateTime.Now.Hour == index)
                        {
                            CurrPrice = Math.Round(price,2);
                        }
                        hourPriceFormated = new HourPriceFormated(price, index);
                        AveragePrice += price;
                        HourPricesFormated.Add(hourPriceFormated);
                        index++;
                    }
                    firstSkipped = true;
                }

                AveragePrice = Math.Round(AveragePrice / 23, 2);
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
        
        public void SetLowPriceLine()
        {
            CurrUserLowPriceDefinition = _settingsService.Settings.lowPriceMark;
            CurrHour = DateTime.Now.Hour + (double) ((double)DateTime.Now.Minute / (double)60);
        }
    }
}
