using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    //public MainPage()
    //{
    //    InitializeComponent();
    //}
}