using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests;

[TestFixture]
public class VoronoiModelTest
{
    [Test]
    public void TestComputeVoronoi()
    {
        const int minX = 0;
        const int maxX = 800;
        const int minY = 0;
        const int maxY = 800;
        var upperLeft = new Point2D(minX, maxY);
        var lowerRight = new Point2D(maxX, minY);
        var points = new[]
        {
            new Point2D(maxX * .2, maxY * .35),
            new Point2D(maxX * .78, maxY * .10),
            new Point2D(maxX * .5, maxY * .6),
            new Point2D(maxX * .3, maxY * .78)
        };

        var voronoiService = new VoronoiService();
        voronoiService.InitBounds(minX, minY, maxX, maxY);
        voronoiService.InitPoints(points);
        voronoiService.ComputeVoronoi();

        SkiaBitmapExportContext skiaBitmapExportContext= new(maxX - minX, maxY - minY, 1);
        var canvas = skiaBitmapExportContext.Canvas;  
        voronoiService.Visualize(canvas);

        foreach (var p in points)
        {
            var pf = new PointF((float)p.X, (float)p.Y);
            canvas.StrokeColor = Colors.Aqua;
            canvas.DrawCircle(pf, 4);
            canvas.FontColor = Colors.Aqua;
            canvas.DrawString($"{p}", (float)(p.X + maxX * 0.03), (float)(p.Y + maxY * 0.03), 
                HorizontalAlignment.Center);
        }
        
        // canvas.StrokeColor = Colors.Ivory;
        // canvas.StrokeSize = 6;
        // canvas.DrawRectangle(minX, minY, maxX - minX, maxY - minY);
        
        // Save the image as a PNG file  
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TestComputeVoronoi.png");
        skiaBitmapExportContext.WriteToFile(path);
    }
}