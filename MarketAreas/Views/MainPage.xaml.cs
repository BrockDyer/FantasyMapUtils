using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using MarketAreas.ViewModels;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MarketAreas.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void DisplayPointInputPopup(View anchor)
    {
        var popup = new Popups.PointInputPopup();
        popup.Anchor = anchor;
        this.ShowPopup(popup);
    }

    private void OnPointsInputClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        DisplayPointInputPopup(button);
    }
}