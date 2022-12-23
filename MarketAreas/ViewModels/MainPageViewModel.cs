using System;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MarketAreas.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly IImageLoadingService _imageLoadingService;

		[ObservableProperty]
		IImage mapImage;

		public MainPageViewModel(IImageLoadingService imageLoadingService)
		{
			_imageLoadingService = imageLoadingService;

			DefaultImageLoad();
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

