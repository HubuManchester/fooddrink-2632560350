using CommunityToolkit.Maui;
using FlavorVault.Services;
using FlavorVault.ViewModels;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FlavorVault;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid-900.ttf", "fa-solid-900");
            });

        // Register Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<FoodEntryRepository>();
        builder.Services.AddSingleton<WishItemRepository>();
        builder.Services.AddSingleton<CollectionRepository>();
        builder.Services.AddSingleton<PlaceMarkRepository>();
        builder.Services.AddSingleton<UserProfileRepository>();
        builder.Services.AddSingleton<SeedDataService>();
        builder.Services.AddSingleton<CatalogCalculator>();
        builder.Services.AddSingleton<FirstRunService>();
        builder.Services.AddSingleton<TextToSpeechService>();
        builder.Services.AddSingleton<SpeechToTextService>();
        builder.Services.AddSingleton<CameraService>();
        builder.Services.AddSingleton<HapticService>();
        builder.Services.AddSingleton<SensorService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<GeolocationService>();
        builder.Services.AddSingleton<DataSeedService>();
        builder.Services.AddSingleton<AuthService>();


        // Register ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<CatalogViewModel>();
        builder.Services.AddTransient<EntryDetailViewModel>();
        builder.Services.AddTransient<EntryEditViewModel>();
        builder.Services.AddTransient<WishListViewModel>();
        builder.Services.AddTransient<ExploreViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<CameraViewModel>();
        builder.Services.AddTransient<CollectionDetailViewModel>();
        builder.Services.AddTransient<NearbyPlacesViewModel>();
        builder.Services.AddTransient<RegionDetailViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<AboutViewModel>();
        builder.Services.AddTransient<ShowcaseManageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        // Register Pages
        builder.Services.AddTransient<Views.HomePage>();
        builder.Services.AddTransient<Views.CatalogPage>();
        builder.Services.AddTransient<Views.EntryDetailPage>();
        builder.Services.AddTransient<Views.EntryEditPage>();
        builder.Services.AddTransient<Views.WishListPage>();
        builder.Services.AddTransient<Views.ExplorePage>();
        builder.Services.AddTransient<Views.ProfilePage>();
        builder.Services.AddTransient<Views.CameraPage>();
        builder.Services.AddTransient<Views.CollectionDetailPage>();
        builder.Services.AddTransient<Views.NearbyPlacesPage>();
        builder.Services.AddTransient<Views.RegionDetailPage>();
        builder.Services.AddTransient<Views.OnboardingPage>();
        builder.Services.AddTransient<Views.AboutPage>();
        builder.Services.AddTransient<Views.ShowcaseManagePage>();
        builder.Services.AddTransient<Views.LoginPage>();
        builder.Services.AddTransient<Views.RegisterPage>();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.AddConsole();
#endif

        return builder.Build();
    }
}
