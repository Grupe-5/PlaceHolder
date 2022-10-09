


using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        
        [ObservableProperty]
        string userName;

        [ObservableProperty]
        string password;
        
        [RelayCommand]
        async Task LogIn()
        {
            
            await Shell.Current.GoToAsync(nameof(HomePage));

            Console.WriteLine(UserName);
            UserName = string.Empty;
            Password = string.Empty;   
        }
    }
}
