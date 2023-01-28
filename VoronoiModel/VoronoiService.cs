using System.Diagnostics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;
using VoronoiLib;
using VoronoiLib.Structures;
using VoronoiModel.Services;
using VoronoiModel.Geometry;
using VoronoiModel.PlanarSubdivision;

namespace VoronoiModel
{
	public class VoronoiService : IVoronoiService
	{
        private List<VoronoiPoint> _points = new();
        private readonly Random _random = new();

		private Point2D? upperLeft;
		private Point2D? lowerRight;
		// private Dcel<Bisector, Point2D>? _dcel;
		private Dcel? _dcel;
		private LinkedList<VEdge>? _voronoiEdges;

		public VoronoiService()
		{
			Debug.WriteLine("Constructing VoronoiService");
		}

		public void AddPoint(VoronoiPoint point)
        {
            _points.Add(point);
            Debug.WriteLine($"Added point {point}");
        }

        public void ComputeVoronoi()
        {
			try
			{
				// _dcel = Algorithm.ComputeVoronoi(upperLeft!, lowerRight!,
				// 	_points.Select(p => new Point2D(p.X.GetValueOrDefault(0), p.Y.GetValueOrDefault(0))));

				var sites = _points.Select(p => new FortuneSite(p.X.GetValueOrDefault(0), p.Y.GetValueOrDefault(0))).ToList();
				_voronoiEdges = FortunesAlgorithm.Run(sites, upperLeft!.X, lowerRight!.Y, lowerRight.X, upperLeft.Y);
				//var voronoiBisectors = voronoiEdges.Select(edge =>
				//{
				//	var bisector = new Bisector();
				//	bisector.Connect(new Point2D(edge.Start.X, edge.Start.Y));
				//	bisector.Connect(new Point2D(edge.End.X, edge.End.Y));
				//	return bisector;
				//});
				Console.WriteLine($"There are {_voronoiEdges.Count} edges in the voronoi diagram");
				_dcel = Dcel.Create(upperLeft, lowerRight, _voronoiEdges.Select(edge =>
				{
					var start = new Point2D(edge.Start.X, edge.Start.Y);
					var end = new Point2D(edge.End.X, edge.End.Y);
					return new LineSegment2D(start, end);
				}).ToList());
				
				// foreach (var face in _dcel.GetFaces())
				// {
				// 	Console.WriteLine(face);
				// }
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
        }

        public List<VoronoiPoint> GetPoints()
        {
            return _points;
        }

        public void InitPoints(double minX, double minY, double maxX, double maxY)
        {
			upperLeft = new Point2D(minX, maxY);
			lowerRight = new Point2D(maxX, minY);
			Console.WriteLine($"Upper Left: {new Point2D(0, 0)}\tLower Right: {new Point2D(maxX, maxY)}");
            foreach (var point in _points)
            {
                point.X = _random.NextDouble() * maxX + minX;
                point.Y = _random.NextDouble() * maxY + minY;
            }
        }

        public void InitPoints(Point2D ul, Point2D lr, params Point2D[] points)
        {
	        upperLeft = ul;
	        lowerRight = lr;
	        _points = points.Select(p => new VoronoiPoint
	        {
		        X = p.X,
		        Y = p.Y
	        }).ToList();
        }

        public void PrintPoints()
        {
            foreach (var p in _points)
            {
                Debug.WriteLine(p);
            }
        }

        public List<Tuple<double, double>> SamplePointsWithinCell(VoronoiPoint point, int sampleSize)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Compute the signed area of the polygon as calculated by the
        /// shoelace formula.
        /// </summary>
        /// <param name="vertices">A list of the point vector vertices. Must
        /// have the start point duplicated at the end.</param>
        /// <returns>The signed area.</returns>
        private static double SignedArea(IReadOnlyList<Point2D> vertices)
        {
	        var sum = 0d;
	        for (var i = 0; i < vertices.Count - 1; i++)
	        {
		        var vi = vertices[i];
		        var vn = vertices[i + 1];

		        sum += (vi.X * vn.Y) - (vn.X * vi.Y);
	        }
	        return 0.5d * sum;
        }

        /// <summary>
		/// Visualize the DCEL. Plot each face using its interior half edges.
		/// </summary>
		/// <param name="canvas">The canvas to draw to.</param>
		/// <exception cref="NotImplementedException"></exception>
		public void Visualize(ICanvas canvas)
        {
			// if (_dcel is null)
			//{
			//	Debug.WriteLine("Dcel is null");
			//	return;
			//};

			if (_voronoiEdges is null)
			{
				Debug.WriteLine("Edges are null");
				return;
			}
			
			// const string color = "#FFFFFF";
			// foreach(var line in _voronoiEdges)
			// {
			// 	var source = new Point2D(line.Start.X, line.Start.Y);
			// 	var target = new Point2D(line.End.X, line.End.Y);
			// 	DrawSegment(source, target, color, canvas);
			// }

			try
			{
				_dcel?.Visualize(canvas);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}

        }

		private static void DrawSegment(Point2D source, Point2D target,
			string color, ICanvas canvas)
		{
			canvas.StrokeColor = Color.FromArgb(color);
			var pointFSource = new PointF((float)source.X, (float)source.Y);
			var pointFTarget = new PointF((float)target.X, (float)target.Y);
			// canvas.DrawString($"{source}", (float)source.X + 5, (float)source.Y + 5, HorizontalAlignment.Center);
			// canvas.DrawString($"{target}", (float)target.X + 5, (float)target.Y + 5, HorizontalAlignment.Center);
			canvas.DrawLine(pointFSource, pointFTarget);
		}
    }
}

