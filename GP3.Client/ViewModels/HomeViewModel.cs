using CommunityToolkit.Mvvm.ComponentModel;
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
        private readonly SettingsService _settingsService;
        public ObservableCollection<HourPriceFormated> HourPricesFormated { get; } = new();

        public HomeViewModel(IPriceApi priceApi, SettingsService settingsService)
        {
            _settingsService = settingsService;
            _priceApi = priceApi;
            Title = "Home";
            GetPricesAsync();

            currHour = DateTime.Now.Hour;

            /* TODO GET this */
            currUserLowPriceDefinition = 300;
        }

        [ObservableProperty]
        public int currUserLowPriceDefinition;

        [ObservableProperty]
        public int currHour;
        async public void GetPricesAsync()
        {
            try
            {
                IsBusy = true;
                var prices = await _priceApi.GetPriceOffsetAsync(DateTime.Today);
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
