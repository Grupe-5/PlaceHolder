using GP3.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GP3.Client.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        readonly AuthService authService;

        public RegisterViewModel(AuthService authService)
        {
            this.authService = authService;
        }

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await authService.RegisterAsync(email, password);
                await Shell.Current.GoToAsync(nameof(HomePage));
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
