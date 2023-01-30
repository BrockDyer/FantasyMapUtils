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
		private const int SampleSize = 50;
		
        private List<VoronoiPoint> _points = new();
        private readonly Random _random = new();

		private Point2D? _upperLeft;
		private Point2D? _lowerRight;
		// private Dcel<Bisector, Point2D>? _dcel;
		private Dcel? _dcel;
		private LinkedList<VEdge>? _voronoiEdges;
		private Dictionary<Point2D, Face> _siteFaceMap;

		public VoronoiService()
		{
			Debug.WriteLine("Constructing VoronoiService");
			_siteFaceMap = new Dictionary<Point2D, Face>();
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
				_voronoiEdges = FortunesAlgorithm.Run(sites, _upperLeft!.X, _lowerRight!.Y, _lowerRight.X, _upperLeft.Y);
				//var voronoiBisectors = voronoiEdges.Select(edge =>
				//{
				//	var bisector = new Bisector();
				//	bisector.Connect(new Point2D(edge.Start.X, edge.Start.Y));
				//	bisector.Connect(new Point2D(edge.End.X, edge.End.Y));
				//	return bisector;
				//});
				// Console.WriteLine($"There are {_voronoiEdges.Count} edges in the voronoi diagram");
				_dcel = Dcel.Create(_upperLeft, _lowerRight, _voronoiEdges.Select(edge =>
				{
					var start = new Point2D(edge.Start.X, edge.Start.Y);
					var end = new Point2D(edge.End.X, edge.End.Y);
					return new LineSegment2D(start, end);
				}).ToList());

				foreach (var site in sites.Select(s => new Point2D(s.X, s.Y)))
				{
					foreach (var face in _dcel.GetFaces().Where(face => face.ContainsPoint(site)))
					{
						_siteFaceMap.Add(site, face);
						// Console.WriteLine($"Voronoi Site at {site} lies within the following face: \n{face}");
						break;
					}
				}
				
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
	        // Console.WriteLine($"Upper Left: {new Point2D(0, 0)}\tLower Right: {new Point2D(maxX, maxY)}");
            foreach (var point in _points)
            {
                point.X = _random.NextDouble() * maxX + minX;
                point.Y = _random.NextDouble() * maxY + minY;
            }
        }

        public void InitBounds(double minX, double minY, double maxX, double maxY)
        {
	        _upperLeft = new Point2D(minX, maxY);
	        _lowerRight = new Point2D(maxX, minY);
        }

        public void InitPoints(params Point2D[] points)
        {
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

        public List<Point2D> SamplePointsWithinCell(Point2D point, int sampleSize)
        {
	        var cell = _siteFaceMap[point];
	        double? minX = null, minY = null, maxX = null, maxY = null;

	        void UpdateBounds(Point2D p)
	        {
		        if (minX is null || p.X < minX) minX = p.X;
		        if (maxX is null || p.X > maxX) maxX = p.X;
		        if (minY is null || p.Y < minY) minY = p.Y;
		        if (maxY is null || p.Y > maxY) maxY = p.Y;
	        }
	        
	        foreach (var edge in cell.GetFaceEdges())
	        {
		        var start = edge.SourceVertex?.Point!;
		        var end = edge.TargetVertex.Point;
		        UpdateBounds(start);
		        UpdateBounds(end);
	        }
	        
	        var result = new List<Point2D>();

	        var sampled = 0;
	        while (sampled < sampleSize)
	        {
		        var xDiffHigh = maxX - minX ?? 1;
		        var xDiffLow = minX - 0 ?? 0;
		        var yDiffHigh = maxY - minY ?? 1;
		        var yDiffLow = minY - 0 ?? 0;
		        var sampleX = _random.NextDouble() * xDiffHigh + xDiffLow;
		        var sampleY = _random.NextDouble() * yDiffHigh + yDiffLow;
		        var samplePoint = new Point2D(sampleX, sampleY);

		        if (!cell.ContainsPoint(samplePoint)) continue;
		        
		        result.Add(samplePoint);
		        sampled += 1;
	        }

	        return result;
        }

        public double Score(List<double> vector)
        {
	        // Init points from the vector.
	        _siteFaceMap = new Dictionary<Point2D, Face>();
	        var points = new List<Point2D>();
	        for (var i = 0; i < vector.Count; i += 2)
	        {
		        points.Add(new Point2D(vector[i], vector[i + 1]));
	        }
	        InitPoints(points.ToArray());
	        
	        // Compute the voronoi diagram.
	        ComputeVoronoi();
	        
	        // Calculate the score.
	        double sumAverageDistances = 0;
	        foreach (var (site, _) in _siteFaceMap)
	        {
		        var sample = SamplePointsWithinCell(site, SampleSize);
		        var distanceSum = sample.Sum(point => Math.Sqrt(Math.Pow(site.X - point.X, 2) + Math.Pow(site.Y - point.Y, 2)));

		        sumAverageDistances += distanceSum / sample.Count;
	        }

	        return -sumAverageDistances;
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

