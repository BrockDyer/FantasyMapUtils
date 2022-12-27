using MarketAreas.Services;

namespace MarketAreas;

public partial class App : Application
{
    public App(IServiceProvider provider)
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}