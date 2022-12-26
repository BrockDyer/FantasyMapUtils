using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoronoiModel;
using VoronoiModel.Services;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MarketAreas.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly IImageLoadingService _imageLoadingService;
		private readonly IVoronoiService _voronoiService;

		[ObservableProperty]
		public IImage mapImage;

		/// <summary>
		/// A collection of voronoi points in the model.
		/// </summary>
		[ObservableProperty]
		List<VoronoiPoint> voronoiPoints;

		public MainPageViewModel(IImageLoadingService imageLoadingService,
			IVoronoiService voronoiService)
		{
			_imageLoadingService = imageLoadingService;
			_voronoiService = voronoiService;

			DefaultImageLoad();
		}

		[RelayCommand]
		void GetPoints()
		{
			VoronoiPoints = _voronoiService.GetPoints();
		}

		/// <summary>
		/// Loads a default image for testing purposes.
		/// </summary>
		private void DefaultImageLoad()
		{
			//Assembly assembly = GetType().GetTypeInfo().Assembly;
			//using (Stream stream = assembly.
			//	GetManifestResourceStream("MarketAreas.Resources.Images.sample_map.png"))
			//{
			//	MapImage = _imageLoadingService.FromStream(stream);
			//}
		}
	}
}

