using Microsoft.Extensions.Logging;

using MarketAreas.Views;
using MarketAreas.ViewModels;
using Microsoft.Maui.Graphics.Platform;

namespace MarketAreas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
//#if WINDOWS
        // Add windows image loader to the builder.
//#else
        builder.Services.AddSingleton<IImageLoadingService, PlatformImageLoadingService>();
//#endif
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();
        return builder.Build();
    }
}