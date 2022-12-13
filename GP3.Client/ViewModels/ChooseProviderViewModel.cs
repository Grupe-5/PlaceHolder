using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using GP3.Client.Models;
using GP3.Common.Entities;
using Java.Security;

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
        async Task GoToTokenPage(string providerString)
        {
            await Shell.Current.GoToAsync($"{nameof(AddAPITokenPage)}", true,
                new Dictionary<string, object>
                {
                {"MeterHistory", MeterHistory},
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
