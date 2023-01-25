using System.Diagnostics;
using Ethereality.DoublyConnectedEdgeList;
using Microsoft.Maui.Graphics;
using VoronoiModel.FortuneVoronoi;
using VoronoiModel.Services;
using VoronoiModel.Geometry;

namespace VoronoiModel
{
	public class VoronoiService : IVoronoiService
	{
        private readonly List<VoronoiPoint> _points = new();
        private readonly Random _random = new();

		private Point2D? upperLeft;
		private Point2D? lowerRight;
		private Dcel<Bisector, Point2D>? _dcel;

		public VoronoiService()
		{
			Debug.WriteLine("Constructing VoronoiService");
		}

		public Dcel<Bisector, Point2D>? Dcel
		{
			get
			{
				return _dcel;
			}
			set
			{
				Debug.WriteLine($"Setting to {value}");
				_dcel = value;
			}
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
				Dcel = Algorithm.ComputeVoronoi(upperLeft!, lowerRight!,
					_points.Select(p => new Point2D(p.X.GetValueOrDefault(0), p.Y.GetValueOrDefault(0))));
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
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
            foreach (var point in _points)
            {
                point.X = _random.NextDouble() * maxX + minX;
                point.Y = _random.NextDouble() * maxY + minY;
            }
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
        /// Compute the centroid of the face.
        /// <see href="https://en.wikipedia.org/wiki/Centroid#Centroid_of_polygon">Formula from Wikipedia</see>
        /// </summary>
        /// <returns>The point vector of the centroid.</returns>
        /// <exception cref="InvalidOperationException"/>
        public Point2D Centroid(IFace<Bisector, Point2D> face)
        {
	        var vertices = new List<Point2D>();
	        var start = face.HalfEdges.First();
	        var current = start;
	        while (true)
	        {
		        if (current is null)
		        {
			        const string msg = "Face is not defined by complete edges.";
			        Debug.WriteLine(msg);
			        throw new InvalidOperationException(msg);
		        }

		        vertices.Add(current.Origin.OriginalPoint);
		        current = current.Next;
		        if (!EqualityComparer<IHalfEdge<Bisector, Point2D>>.Default.Equals(current, start)) continue;
				
		        // Need to have the start vertex in twice.
		        vertices.Add(current.Origin.OriginalPoint);
		        break;
	        }

	        var area = SignedArea(vertices);
	        var cxSum = 0d;
	        var cySum = 0d;
	        for (var i = 0; i < vertices.Count - 1; i++)
	        {
		        var vi = vertices[i];
		        var vn = vertices[i + 1];
		        cxSum += (vi.X + vn.X) * ((vi.X * vn.Y) - (vn.X * vi.Y));
		        cySum += (vi.Y + vn.Y) * ((vi.X * vn.Y) - (vn.X * vi.Y));
	        }

	        var cx = (1d / (6d * area)) * cxSum;
	        var cy = (1d / (6d * area)) * cySum;

	        return new Point2D(Math.Round(cx, 3), Math.Round(cy, 3));
        }

        /// <summary>
		/// Visualize the DCEL. Plot each face using its interior half edges.
		/// </summary>
		/// <param name="canvas">The canvas to draw to.</param>
		/// <exception cref="NotImplementedException"></exception>
		public void Visualize(ICanvas canvas)
        {
	        if (Dcel is null)
			{
				Debug.WriteLine("Dcel is null");
				return;
			};
	        
			var colors = new[] {
				"#cc6666", "#997a00", "#00f261", "#408cff", "#584359", "#d9a3a3", "#bfb68f",
				"#4d665a", "#1a3866", "#f200c2", "#4c1400", "#f2e63d", "#1d7356", "#bfd9ff",
				"#ffbff2", "#f26d3d", "#57661a", "#bffff2", "#3939e6", "#ff408c", "#664733",
				"#bfff40", "#00f2e2", "#1b134d", "#731d3f", "#995200", "#eaffbf", "#2daab3",
				"#7453a6", "#b20018", "#f29d3d", "#12330d", "#004759", "#bf40ff", "#330007",
				"#4c3913", "#65b359", "#0077b3", "#530059"
			};

			// const string emptyFaceColor = "#530059";

            const double epsilon = 0.01;
            Dictionary<IFace<Bisector, Point2D>, int> faceIndexMap = new();
            Dictionary<IFace<Bisector, Point2D>, Point2D> faceCentroidMap = new();

			var i = 0;
			foreach(var face in Dcel.Faces)
			{
				faceIndexMap[face] = i;
				faceCentroidMap[face] = Centroid(face);
				i += 1;
			}

			HashSet<IHalfEdge<Bisector, Point2D>> drawn = new();
			foreach(var edge in Dcel.HalfEdges)
			{
				// Already drawn
				if (drawn.Contains(edge)) continue;

				// Draw the edge.
				var twin = edge.Twin;

				var target = twin.Origin.OriginalPoint;
				var source = edge.Origin.OriginalPoint;

				const int direction = 1;
				var face = edge.Face;
				var twinFace = twin.Face;

				var centroid = faceCentroidMap[face];

				var faceColor = colors[faceIndexMap[face] % colors.Length];
				var twinFaceColor = colors[faceIndexMap[twinFace] % colors.Length];

				DrawSegment(source, target, centroid, direction * epsilon, faceColor, canvas);
				DrawSegment(twin.Origin.OriginalPoint, twin.Origin.OriginalPoint,
					centroid, -1 * direction * epsilon, twinFaceColor, canvas);

                // Update drawn set.
                drawn.Add(edge);
				drawn.Add(twin);
			}
		}

		private static void DrawSegment(Point2D source, Point2D target, Point2D centroid,
			double epsilon, string color, ICanvas canvas)
		{
			canvas.StrokeColor = Color.FromArgb(color);
			var shiftedSource = ShiftPointTowardPoint(source, centroid, epsilon);
			var shiftedTarget = ShiftPointTowardPoint(target, centroid, epsilon);
			var pointFSource = new PointF((float)shiftedSource.X, (float)shiftedSource.Y);
			var pointFTarget = new PointF((float)shiftedTarget.X, (float)shiftedTarget.Y);
			canvas.DrawLine(pointFSource, pointFTarget);
		}

		/// <summary>
		/// Compute a point that is slightly shifted in a straight line towards
		/// another point by some factor epsilon.
		/// </summary>
		/// <param name="p1">The source point.</param>
		/// <param name="p2">The destination point.</param>
		/// <param name="epsilon">The shift scale.</param>
		/// <returns>A point along the line p1 -> p2 scaled by epsilon.</returns>
		private static Point2D ShiftPointTowardPoint(Point2D p1, Point2D p2, double epsilon)
		{
			var dx = p2.X - p1.X;
			var dy = p2.Y - p1.Y;
			if (dx == 0)
				return new Point2D(p1.X, p1.Y + (Math.Sign(dy) * epsilon));

			return dy == 0 ? new Point2D(p1.X + (Math.Sign(dx) * epsilon), p1.Y) : 
				new Point2D(p1.X + (dx * epsilon), p1.Y + (dy * epsilon));
		}
    }
}

