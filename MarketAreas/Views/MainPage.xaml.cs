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

public partial class MainPage
{
    //private readonly IVoronoiService _voronoiService;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        viewModel.InvalidateVisualization = InvalidateVisualizationView;
        BindingContext = viewModel;
        //_voronoiService = voronoiService;
    }

    // Used to invalidate the graphics view for the visualization.
    // TODO: Determine if there is a cleaner way to do this (probably yes).
    private void InvalidateVisualizationView()
    {
        VisualizationView.Invalidate();
    }
}