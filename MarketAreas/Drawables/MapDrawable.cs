using System;
using VoronoiModel;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MarketAreas.Drawables
{
	public class MapDrawable : BindableObject, IDrawable
	{
        /// <summary>
        /// The map image the user is editing.
        /// </summary>
        public IImage MapImage
        {
            get => (IImage)GetValue(MapImageProperty);
            set => SetValue(MapImageProperty, value);
        }

        public static readonly BindableProperty MapImageProperty =
            BindableProperty.Create(nameof(MapImage), typeof(IImage), typeof(MapDrawable));

        /// <summary>
        /// The spacing around the image and its container.
        /// </summary>
        public int MapImageMargin
        {
            get => (int)GetValue(MapImageMarginProperty);
            set => SetValue(MapImageMarginProperty, value);
        }

        public static readonly BindableProperty MapImageMarginProperty =
            BindableProperty.Create(nameof(MapImageMargin), typeof(int), typeof(MapDrawable));

        /// <summary>
        /// The points to draw from the voronoi model.
        /// </summary>
        public List<VoronoiPoint> VoronoiPoints
        {
            get => (List<VoronoiPoint>)GetValue(VoronoiPointsProperty);
            set => SetValue(VoronoiPointsProperty, value);
        }

        public static BindableProperty VoronoiPointsProperty =
            BindableProperty.Create(nameof(VoronoiPoints), typeof(List<VoronoiPoint>), typeof(MapDrawable));

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

            // Draw the image.
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

                canvas.DrawImage(newImage, ix, iy, iw, ih);
            }

            // Draw voronoi points
            DrawPoints(canvas);

            // Draw voronoi lines and cells

            // Draw optimal points
        }

        private void DrawPoints(ICanvas canvas)
        {
            foreach (var point in VoronoiPoints)
            {
                if (point.GetX() is not null && point.GetY() is not null)
                {
                    var cx = (float)point.GetX();
                    var cy = (float)point.GetY();
                    var center = new PointF(cx, cy);
                    //canvas.StrokeColor = point.Color;
                    canvas.DrawCircle(center, 4);
                }
            }
        }
    }
}

