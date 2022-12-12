using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using GP3.Client.Models;

namespace GP3.Client.ViewModels
{
    [QueryProperty("MeterHistory", "MeterHistory")]
    public partial class ChooseProviderViewModel : BaseViewModel
    {
        private readonly UsageDataService _historyService;
        public ChooseProviderViewModel(UsageDataService historyService)
        {
            _historyService = historyService;
            
        }
        [ObservableProperty]
        double currentDraw;

        
        [ObservableProperty]
        public ObservableCollection<MeterHistory> meterHistory;

        [RelayCommand]
        async Task GoToTokenPage(string provider)
        {
            MeterHistory mh = meterHistory.First();
            mh.monthEstCost = 36; // DELTE ME

            switch(provider)
            {
                case "Eso":
                    mh.electricityProvider = ElectricityProviders.Eso;
                    break;
                case "Ignitis":
                    mh.electricityProvider = ElectricityProviders.Ignitis;
                    break;
                case "Perlas":
                    mh.electricityProvider = ElectricityProviders.Perlas;
                    break;
            }    
            
            await Shell.Current.GoToAsync($"{nameof(AddAPITokenPage)}", true,
                new Dictionary<string, object>
                {
                {"MeterHistory", MeterHistory}
                });
        }
    }
}
