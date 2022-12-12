using CommunityToolkit.Mvvm.ComponentModel;
using DevExpress.Maui.Core.Internal;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Client.Services;
using System.Collections.ObjectModel;
using GP3.Client.CustomControls;

namespace GP3.Client.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPriceApi _priceApi;
        private readonly SettingsService _settingsService;
        public ObservableCollection<HourPriceFormated> HourPricesFormated { get; } = new();

        public HomeViewModel(IPriceApi priceApi, SettingsService settingsService)
        {
            _priceApi = priceApi;
            Title = "Home";
            GetPricesAsync();

            currHour = DateTime.Now.Hour;

            UserSettings us = settingsService.GetSettings1();
            if(us != null)
            {
                currUserLowPriceDefinition = us.lowPriceMark;
            }
            else
            {
                currUserLowPriceDefinition = 250;
            }

        }

        [ObservableProperty]
        public double currUserLowPriceDefinition;

        [ObservableProperty]
        public int currHour;

        [ObservableProperty]
        public DateTime currentDate;
        async public void GetPricesAsync()
        {
            try
            {
                IsBusy = true;
                var prices = await _priceApi.GetPriceOffsetAsync(CurrentDate);
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
