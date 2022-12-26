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
    private readonly IVoronoiService _voronoiService;

    public MainPage(MainPageViewModel viewModel, IVoronoiService voronoiService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _voronoiService = voronoiService;
    }

    private void DisplayPointInputPopup(View anchor)
    {
        var pointInputPopup = new PointInputPopup(_voronoiService);
        pointInputPopup.Anchor = anchor;
        this.ShowPopup(pointInputPopup);
    }

    private void OnPointsInputClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        DisplayPointInputPopup(button);
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        // Initialize the voronoi centroids.
        double x, y, width, height;
        VisualizationView.Bounds.Deconstruct(out x, out y, out width, out height);
        _voronoiService.InitPoints(x, y, x + width, y + height);

        _voronoiService.PrintPoints();
        VisualizationView.Invalidate();
    }
}