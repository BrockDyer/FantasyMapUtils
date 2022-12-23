using System;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MarketAreas.Drawables
{
	public class MapDrawable : BindableObject, IDrawable
	{

        public IImage MapImage
        {
            get => (IImage)GetValue(MapImageProperty);
            set => SetValue(MapImageProperty, value);
        }

        public static readonly BindableProperty MapImageProperty =
            BindableProperty.Create(nameof(MapImage), typeof(IImage), typeof(MapDrawable));

        public int MapImageMargin
        {
            get => (int)GetValue(MapImageMarginProperty);
            set => SetValue(MapImageMarginProperty, value);
        }

        public static readonly BindableProperty MapImageMarginProperty =
            BindableProperty.Create(nameof(MapImageMargin), typeof(int), typeof(MapDrawable));

        /// <summary>
        /// Construct a MapDrawable.
        /// </summary>
        public MapDrawable()
        {
            MapImageMargin = 0;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Purple;
            canvas.StrokeSize = 4;
            var width = dirtyRect.Right - dirtyRect.Left;
            var height = dirtyRect.Bottom - dirtyRect.Top;
            canvas.DrawRectangle(dirtyRect.Left, dirtyRect.Top, width, height);

            if (MapImage != null)
            {
                // Resize the image so that it fits within the box.
                IImage newImage = MapImage;
                if (MapImage.Width > width || MapImage.Height > height)
                    newImage = MapImage.Downsize(Math.Min(width, height), false);

                // Compute the (x, y) coord to start drawing this image at.
                var ix = (width - newImage.Width) / 2 + MapImageMargin;
                var iy = (height - newImage.Height) / 2 + MapImageMargin;

                // Compute the image width to draw (by subtracting margins)
                var iw = newImage.Width - (2 * MapImageMargin);
                var ih = newImage.Height - (2 * MapImageMargin);

                // Draw the image.
                canvas.DrawImage(newImage, ix, iy, iw, ih);
            }
        }
    }
}

