using Android.OS;
using DevExpress.Maui;
using GP3.Client.Refit;
using GP3.Client.Services;
using GP3.Client.ViewModels;
using MonkeyCache;
using MonkeyCache.FileStore;
using Sentry;

namespace GP3.Client;

public static class MauiProgram
{
    
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSentry(options =>
            {
                // The DSN is the only required setting.
                options.Dsn = "https://369041dcedd24381ad160911a69b287c@o4504267915264000.ingest.sentry.io/4504293859852288";

            })
            .UseDevExpress()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Bold.ttf", "OpenSansBold");
                fonts.AddFont("Sitka.ttc", "Sitka");
            });

        /* Pages and viewmodels should be transient */
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<RegisterViewModel>();

        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<HistoryViewModel>();

        builder.Services.AddTransient<HistoryMonthAddViewModel>();
        builder.Services.AddTransient<HistoryMonthAddPage>();

        builder.Services.AddTransient<StatisticsPage>();
        builder.Services.AddTransient<StatisticsViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<HomeViewModel>();

        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<SettingsViewModel>();

        builder.Services.AddTransient<IntegrationsPage>();
        builder.Services.AddTransient<IntegrationsViewModel>();

        builder.Services.AddTransient<EditDevicePage>();
        builder.Services.AddTransient<EditDeviceViewModel>();

        builder.Services.AddTransient<AddDevicePage>();
        builder.Services.AddTransient<AddDeviceViewModel>();

        /* Services */
        builder.Services.AddSingleton<HistoryService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<SettingsService>();

        /* TODO: Replace with actual connectivity service */
        builder.Services.AddSingleton<IConnectivityService, VoidConnectivityService>();

        builder.Services.AddSingleton<IBarrel>(_ => {
            Barrel.ApplicationId = "gp3.client";
            return Barrel.Current;
        });

        /* TODO: Make these options configurable */
        var apiRetryCount = 3;
        var apiRetryWait = TimeSpan.FromSeconds(1);
        var apiTimeout = TimeSpan.FromSeconds(10);
        builder.Configuration["ApiURI"] = "https://grupe3.azurewebsites.net/";

        builder.Services
            .AddResilientApi<IPriceApi>(builder.Configuration["ApiURI"], apiRetryCount, apiRetryWait, apiTimeout)
            .AddResilientApi<IIntegrationApi>(builder.Configuration["ApiURI"], apiRetryCount, apiRetryWait, apiTimeout);

        builder.Services.Decorate<IPriceApi, CachedPriceApi>();

        return builder.Build();
    }
}
