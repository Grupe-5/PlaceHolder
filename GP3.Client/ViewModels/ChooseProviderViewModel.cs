using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using GP3.Client.Models;
using GP3.Common.Entities;


namespace GP3.Client.ViewModels
{
    [QueryProperty("MeterHistoryCollection", "MeterHistoryCollection")]
    public partial class ChooseProviderViewModel : BaseViewModel
    {
        public ChooseProviderViewModel()
        {
            
        }
        [ObservableProperty]
        double currentDraw;

        [ObservableProperty]
        public ObservableCollection<MeterHistory> meterHistoryCollection;
                                                  
        [RelayCommand]
        async Task GoToTokenPage(string providerString)
        {

            await Shell.Current.GoToAsync($"{nameof(AddAPITokenPage)}", true,
                new Dictionary<string, object>
                {
                {"MeterHistoryCollection", MeterHistoryCollection},
                {"Provider", GetProvider(providerString) }
                });
        }

        private ProviderSelection GetProvider(string providerString)
        {
            switch (providerString)
            {
                case "Eso":
                    return ProviderSelection.Eso;   
                case "Ignitis":
                    return ProviderSelection.Ignitis;
                case "Perlas":
                    return ProviderSelection.Perlas;
                default:
                    return ProviderSelection.Perlas;
            }
        }
    }
}
