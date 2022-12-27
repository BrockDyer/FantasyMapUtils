using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using IImage = Microsoft.Maui.Graphics.IImage;

using MarketAreas.ViewModels;
using MarketAreas.Views.Popups;
using VoronoiModel.Services;

namespace MarketAreas.Views;

public partial class MainPage : ContentPage
{
    //private readonly IVoronoiService _voronoiService;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        //_voronoiService = voronoiService;
    }

    // This is clunky (and feels like it's not how MVVM is supposed to work).
    // TODO: Find a cleaner way (or verify that this is the cleanest).
    public void OnStartClicked(object sender, EventArgs e)
    {
        VisualizationView.Invalidate();
    }
}