using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics.Platform;
using CommunityToolkit.Maui;

using MarketAreas.Views;
using MarketAreas.Views.Popups;
using MarketAreas.ViewModels;
using VoronoiModel;
using VoronoiModel.Services;
using MarketAreas.Services;

namespace MarketAreas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
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
        builder.Services.AddSingleton<IPopupService, PopupService>();
        builder.Services.AddSingleton<IVoronoiService, VoronoiService>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}