using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VoronoiModel;
using VoronoiModel.Services;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MarketAreas.Drawables
{
	public class VisualizationDrawable : BindableObject, IDrawable
	{
        private readonly IVoronoiService _voronoiService;

        private float x, y, width, height;

        /// <summary>
        /// Construct a MapDrawable.
        /// </summary>
        public VisualizationDrawable(IVoronoiService voronoiService)
        {
            _voronoiService = voronoiService;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            dirtyRect.Deconstruct(out x, out y, out width, out height);
            canvas.StrokeColor = Colors.Purple;
            canvas.StrokeSize = 4;
            canvas.DrawRectangle(dirtyRect.Left, dirtyRect.Top, width, height);

            // Draw the image.
            //if (MapImage != null)
            //{
            //    // Resize the image so that it fits within the box.
            //    IImage newImage = MapImage;
            //    if (MapImage.Width > width || MapImage.Height > height)
            //        newImage = MapImage.Downsize(Math.Min(width, height), false);

            //    // Compute the (x, y) coord to start drawing this image at.
            //    var ix = (width - newImage.Width) / 2 + MapImageMargin;
            //    var iy = (height - newImage.Height) / 2 + MapImageMargin;

            //    // Compute the image width to draw (by subtracting margins)
            //    var iw = newImage.Width - (2 * MapImageMargin);
            //    var ih = newImage.Height - (2 * MapImageMargin);

            //    canvas.DrawImage(newImage, ix, iy, iw, ih);
            //}

            // Draw voronoi points
            DrawPoints(canvas);

            // Draw voronoi lines and cells

            // Draw optimal points
        }

        /// <summary>
        /// Get the dimensions of the canvas.
        /// </summary>
        /// <returns>A 4 Tuple containing the x, y, width, height. Where (x,y)
        /// is the top left point of the canvas.</returns>
        public Tuple<float, float, float, float> GetCanvasSize()
        {
            return Tuple.Create(x, y, width, height);
        }

        private void DrawPoints(ICanvas canvas)
        {
            foreach (var point in _voronoiService.GetPoints())
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

