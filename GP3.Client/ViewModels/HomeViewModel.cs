using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GP3.Client.Models;
using GP3.Client.Refit;
using GP3.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GP3.Client.ViewModels.HomeViewModel;

namespace GP3.Client.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPriceApi _priceApi;

        public ObservableCollection<HourPriceFormated> HourPricesFormated { get; } = new();

        public HomeViewModel(IPriceApi priceApi)
        {
            Title = "Home";
            _priceApi = priceApi;
            GetPricesAsync();
        }

        async public void GetPricesAsync()
        {
            try
            {
                IsBusy = true;

                var prices = await _priceApi.GetPriceAsync(DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));

                int index = 0;
                HourPriceFormated hourPriceFormated;
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
