using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IImage = Microsoft.Maui.Graphics.IImage;

using VoronoiModel;
using VoronoiModel.Services;
using MarketAreas.Views.Popups;
using MarketAreas.Services;
using OptimizationLib;
using VoronoiModel.Geometry;

namespace MarketAreas.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly IImageLoadingService _imageLoadingService;
		private readonly IVoronoiService _voronoiService;
		private readonly IOptimizationAlgorithm _optimizationAlgorithm;
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
            IPopupService popupService,
			IOptimizationAlgorithm optimizationAlgorithm)
		{
			_imageLoadingService = imageLoadingService;
			_voronoiService = voronoiService;
            _popupService = popupService;
            _optimizationAlgorithm = optimizationAlgorithm;

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
	        try
	        {
		        // Initialize the voronoi region.
		        var (minX, minY, item3, item4) = VisualizationDrawable.GetCanvasSize();
		        var maxX = item3 + minX;
		        var maxY = item4 + minY;
		        _voronoiService.InitBounds(minX, minY, maxX, maxY);
		        // _voronoiService.InitPoints((double)canvasDims.Item1,
		        //     (double)canvasDims.Item2,
		        //     (double)(canvasDims.Item3 + canvasDims.Item1),
		        //     (double)(canvasDims.Item4 + canvasDims.Item2));

		        // _voronoiService.PrintPoints();
		        // _voronoiService.ComputeVoronoi();

		        // Perform the optimization
		        var minBounds = new List<double>();
		        var maxBounds = new List<double>();
		        for (var i = 0; i < _voronoiService.GetPoints().Count * 2; i += 2)
		        {
			        minBounds.Add(minX);
			        minBounds.Add(minY);
			        
			        maxBounds.Add(maxX);
			        maxBounds.Add(maxY);
		        }

		        var solution = _optimizationAlgorithm.Solve(new Random(), minBounds, maxBounds, _voronoiService.Score);
		        var solutionPoints = new List<Point2D>();
		        for (var i = 0; i < solution.Count; i += 2)
		        {
			        solutionPoints.Add(new Point2D(solution[i], solution[i + 1]));
		        }
		        
		        Console.Write("Solution found at: ");
		        foreach (var point in solutionPoints)
		        {
			        Console.Write($"{point} ");
		        }
		        Console.WriteLine($"\nSolution score: {_voronoiService.Score(solution)}");

		        _voronoiService.InitPoints(solutionPoints.ToArray());
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine(e.Message);
		        Console.WriteLine(e.StackTrace);
	        }

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

