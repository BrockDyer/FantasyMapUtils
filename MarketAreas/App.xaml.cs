namespace MarketAreas;

public partial class App
{
    public App(IServiceProvider provider)
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}