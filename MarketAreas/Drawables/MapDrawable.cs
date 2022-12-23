using System;
namespace MarketAreas.Drawables
{
	public class MapDrawable : IDrawable
	{

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Purple;
            canvas.StrokeSize = 4;
            var width = dirtyRect.Right - dirtyRect.Left;
            var height = dirtyRect.Bottom - dirtyRect.Top;
            canvas.DrawRectangle(dirtyRect.Left, dirtyRect.Top, width, height);
        }
    }
}

