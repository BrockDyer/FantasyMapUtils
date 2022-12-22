namespace MarketAreas.Views;

public partial class TemplatePage : ContentPage
{
    int count = 0;

    public TemplatePage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        CounterBtn.Text = count == 1 ? $"Clicked {count} time" : $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}