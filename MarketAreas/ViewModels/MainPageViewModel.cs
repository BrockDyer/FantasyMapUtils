using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IImage = Microsoft.Maui.Graphics.IImage;

using VoronoiModel;
using VoronoiModel.Services;
using MarketAreas.Views.Popups;
using MarketAreas.Services;

namespace MarketAreas.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly IImageLoadingService _imageLoadingService;
		private readonly IVoronoiService _voronoiService;
        private readonly IPopupService _popupService;

        /// <summary>
        /// The map to display in the background.
        /// </summary>
		[ObservableProperty]
		private IImage mapImage;

        /// <summary>
        /// A collection of voronoi points in the model.
        /// </summary>
        private ObservableCollection<VoronoiPoint> VoronoiPoints { get; set; } = new();

        public Drawables.VisualizationDrawable VisualizationDrawable { get; }
        public Action InvalidateVisualization { get; set; }

		public MainPageViewModel(IImageLoadingService imageLoadingService,
			IVoronoiService voronoiService,
            IPopupService popupService)
		{
			_imageLoadingService = imageLoadingService;
			_voronoiService = voronoiService;
            _popupService = popupService;

            VisualizationDrawable = new Drawables.VisualizationDrawable(_voronoiService);
		}

        /// <summary>
        /// Display a popup to collect information about a new voronoi point.
        /// </summary>
        /// <param name="anchor">The element to anchor the popup to.</param>
        [RelayCommand]
        private void DisplayPointInputPopup(View anchor)
        {
	        var pointInputPopup = new PointInputPopup(AddVoronoiPoint)
            {
	            Anchor = anchor
            };
            _popupService.ShowPopup(pointInputPopup);
        }

        /// <summary>
        /// Handle the Start button being clicked.
        /// </summary>
        [RelayCommand]
        private void Start()
        {
	        // Initialize the voronoi centroids.
            var canvasDims = VisualizationDrawable.GetCanvasSize();
            _voronoiService.InitPoints((double)canvasDims.Item1,
                (double)canvasDims.Item2,
                (double)(canvasDims.Item3 + canvasDims.Item1),
                (double)(canvasDims.Item4 + canvasDims.Item2));

            _voronoiService.PrintPoints();
            _voronoiService.ComputeVoronoi();
            InvalidateVisualization();
        }

        // This feels clunky.
        // TODO: Find a way to consolidate this properly.
        private void AddVoronoiPoint(VoronoiPoint point)
        {
            _voronoiService.AddPoint(point);
            VoronoiPoints.Add(point);
        }
    }
}

