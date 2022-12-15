using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Maui.Core.Internal;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Client.Services;
using System.Collections.ObjectModel;

namespace GP3.Client.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPriceApi _priceApi;
        public ObservableCollection<HourPriceFormated> HourPricesFormated { get; } = new();

        private DateTime today;

        public HomeViewModel(IPriceApi priceApi, SettingsService settingsService)
        {
            _priceApi = priceApi;
            Title = "Home";

            today = DateTime.Now.Date;
            selectedDate = today;
            selectedDateFormated = selectedDate.ToShortDateString();

            currHour = DateTime.Now.Hour;
            GetPricesAsync();

            currUserLowPriceDefinition = settingsService.Settings.lowPriceMark;
        }

        [ObservableProperty]
        public double currUserLowPriceDefinition;

        [ObservableProperty]
        public int currHour;

        [ObservableProperty]
        public string selectedDateFormated;

        [ObservableProperty]
        public DateTime selectedDate;

        [RelayCommand]
        void DecreaseDate()
        {
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
                SelectedDateFormated = SelectedDate.ToShortDateString();
                GetPricesAsync();
            }
            else
            {
                Shell.Current.DisplayAlert("Datos keisti negalima!", "Mes nezinom kokios kainos but rytoj", "OK");
            }
        }

        async public void GetPricesAsync()
        {
            try
            {
                IsBusy = true;
                var prices = await _priceApi.GetPriceOffsetAsync(SelectedDate);
                int index = 0;
                HourPriceFormated hourPriceFormated;
                HourPricesFormated.Clear();
                foreach (var price in prices.HourlyPrices)
                {
                    hourPriceFormated = new HourPriceFormated(price, index);
                    HourPricesFormated.Add(hourPriceFormated);
                    index++;
                }
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
