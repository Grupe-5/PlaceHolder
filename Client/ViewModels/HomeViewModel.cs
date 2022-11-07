
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Client.Services;

namespace Client.ViewModels
{
    public partial class HomeViewModel: BaseViewModel 
    {
        HomeService HomeService;
        public ObservableCollection<Double> DayPrices { get; } = new();

        public HomeViewModel(HomeService HomeService)
        {
            this.HomeService = HomeService;
        }

        [ObservableProperty]
        string text = "Nice";

        [RelayCommand]
        void Nice()
        {


        }
    }

}

