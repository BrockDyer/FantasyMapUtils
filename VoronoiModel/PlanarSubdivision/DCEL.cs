using System;
using System.Diagnostics;
using Microsoft.Maui.Graphics;
using VoronoiModel.Geometry;
using Point = VoronoiModel.Geometry.Point;

namespace VoronoiModel.PlanarSubdivision
{
	/// <summary>
	/// An exception that is raised when attempting to split a half edge by
	/// a new vertex and the vertex does not fall on that half edge.
	/// </summary>
	public class PointNotOnHalfEdgeException : Exception { }

	/// <summary>
	/// An exception that is raised when attempting to perform an operation with
	/// an existing vertex u, but u does not exist.
	/// </summary>
	public class VertexNotFoundException : Exception { }

	/// <summary>
	/// A subdivision of a planar space into faces. Faces may be traversed by
	/// following the next and previous properties of any edge incident to that
	/// face. Adjacent faces can be navigated to via the twin of the corresponding
	/// half edge on the current face.
	/// </summary>
	public class DCEL
	{
		//HashSet<HalfEdge> HalfEdges { get; } = new();
		//HashSet<Vertex> Vertices { get; } = new();

		/// <summary>
		/// Efficiently store half edges as Source -> Target -> HalfEdge
		/// </summary>
		Dictionary<Point, Dictionary<Point, HalfEdge>> HalfEdges { get; } = new();
		Dictionary<Point, Vertex> Vertices { get; } = new();
        HashSet<Face> Faces { get; } = new();

		/// <summary>
		/// Create a new instance of the DCEL object.
		/// </summary>
		private DCEL() {}

		private HalfEdge GetEdge(Point source, Point target)
		{
			return HalfEdges[source][target];
		}

		private Vertex GetVertex(Point p)
		{
			return Vertices[p];
		}

		/// <summary>
		/// Create a doubly connected edge list to represent a planar subdivision
		/// of the plane given by the upper left point and the lower right point.
		/// </summary>
		/// <param name="upperLeft">The upper left point of the plane.</param>
		/// <param name="lowerRight">The lower right point of the plane.</param>
		/// <returns>A DCEL initialized such that the only face is the plane.</returns>
		public static DCEL Create(Point upperLeft, Point lowerRight)
		{
			var dcel = new DCEL();

            // Construct vertices.
            var vul = new Vertex(upperLeft.X, upperLeft.Y);
            var vur = new Vertex(lowerRight.X, upperLeft.Y);
            var vlr = new Vertex(lowerRight.X, lowerRight.Y);
            var vll = new Vertex(upperLeft.X, lowerRight.Y);

            // Construct half edges
            var halfEdges = new List<HalfEdge>();
            foreach (var vertex in new Vertex[] { vul, vur, vlr, vll, vll, vul, vur, vlr })
            {
                var edge = new HalfEdge(vertex);
                halfEdges.Add(edge);
                vertex.Edge = edge;
            }

            // Construct face
            var face = new Face(halfEdges[0]);

            // Post-construction initialization of half edges.
            for (var i = 0; i < 4; i++)
            {
                var h1 = halfEdges[i];
                var h2 = halfEdges[i + 4];

                // Set twins
                //h1.Twin = h2;
                //h2.Twin = h1;
				h1.LinkTwin(h2);

                // Set line segments
                h1.Segment = new LineSegment(h1.SourceVertex.Point, h1.TargetVertex.Point);
                h2.Segment = new LineSegment(h2.SourceVertex.Point, h2.TargetVertex.Point);

                // Set previous and next
                var prevIndex = (i + 4) % 4;
                var nextIndex = (i + 1) % 4;
                h1.Previous = halfEdges[prevIndex];
                h1.Next = halfEdges[nextIndex];

                h2.Previous = halfEdges[4 + prevIndex];
                h2.Next = halfEdges[4 + nextIndex];

                // Set the face (only for interior edges)
                h1.IncidentFace = face;
            }

            dcel.Faces.Add(face);
            foreach(var edge in halfEdges)
			{
				var source = edge.SourceVertex?.Point;
				var target = edge.TargetVertex.Point;

				if (source is null)
					continue;

				if (!dcel.HalfEdges.ContainsKey(source))
				{
					dcel.HalfEdges.Add(source, new());
				}

				dcel.HalfEdges[source].Add(target, edge);
			}

			foreach(var vertex in new Vertex[] {vul, vur, vlr, vll })
			{
				dcel.Vertices.Add(vertex.Point, vertex);
			}

			return dcel;
        }

		/// <summary>
		/// Add a vertex at point v1 that is connected to the vertex at point v2.
		/// v2 must be an existing vertex.
		/// </summary>
		/// <param name="v1">The new vertex to add as a point vector.</param>
		/// <param name="v2">The existing vertex to connect v1 to as a point vector.</param>
		public void AddVertex(Point v1, Point v2)
		{
			var existingVertex = GetVertex(v2);
			throw new NotImplementedException();
		}

        /// <summary>
        /// Split a given half edge by adding a new vertex.
        /// </summary>
        /// <param name="v">The vertex to add as a point vector.</param>
        /// <param name="source">The source vertex of the half edge to split.</param>
        /// <param name="target">The target vertex of the half edge to split.</param>
        public void SplitEdge(Point v, Point source, Point target)
		{
			var h = GetEdge(source, target);
			throw new NotImplementedException();
		}

		/// <summary>
		/// Split a face by connecting two vertices on the face Returns a tuple
		/// of the newly created faces.
		/// </summary>
		/// <param name="v">The first vertex as a point vector.</param>
		/// <param name="u">The second vertex as a point vector.</param>
		/// <returns>A tuple containing the two newly created faces.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public Tuple<Face, Face> SplitFace(Point v, Point u)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete a given half edge, thus merging two faces. Returns the newly
		/// merged face.
		/// </summary>'
		/// <param name="source">The source vertex of the edge to delete.</param>
		/// <param name="target">The target vertex of the edge to delete.</param>
		/// <returns>The new face.</returns>
		public Face DeleteEdge(Point source, Point target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes a given vertex, thus merging incident half edges. This can
		/// only be done if the number of incident edges to this vertex is less
		/// than or equal to four.
		/// </summary>
		/// <param name="v">The vertex to delete as a point vector.</param>
		/// <exception cref="NotImplementedException"></exception>
		public void DeleteVertex(Point v)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Visualize the DCEL. Plot each face using its interior half edges.
		/// </summary>
		/// <param name="canvas">The canvas to draw to.</param>
		/// <exception cref="NotImplementedException"></exception>
		public void Visualize(ICanvas canvas)
		{
			var colors = new string[] {
				"#cc6666", "#997a00", "#00f261", "#408cff", "#584359", "#d9a3a3", "#bfb68f",
				"#4d665a", "#1a3866", "#f200c2", "#4c1400", "#f2e63d", "#1d7356", "#bfd9ff",
				"#ffbff2", "#f26d3d", "#57661a", "#bffff2", "#3939e6", "#ff408c", "#664733",
				"#bfff40", "#00f2e2", "#1b134d", "#731d3f", "#995200", "#eaffbf", "#2daab3",
				"#7453a6", "#b20018", "#f29d3d", "#12330d", "#004759", "#bf40ff", "#330007",
				"#4c3913", "#65b359", "#0077b3"
			};

			var emptyFaceColor = "#530059";

            var epsilon = 0.01M;
            Dictionary<Face, int> faceIndexMap = new();
            Dictionary<Face, Point> faceCentroidMap = new();

			int i = 0;
			foreach(var face in Faces)
			{
				faceIndexMap[face] = i;
				faceCentroidMap[face] = face.Centroid();
				i += 1;
			}

			HashSet<HalfEdge> drawn = new();
			foreach(var edge in HalfEdges.Values.SelectMany(map => map.Values))
			{
				// Already drawn
				if (drawn.Contains(edge)) continue;

				// Draw the edge.
				var twin = edge.Twin;
				if (twin is null)
				{
					Debug.WriteLine("Encountered a half edge without a twin in a constructed DCEL", "Error");
					continue;
				}

                var target = edge.TargetVertex.Point;
				var source = edge.SourceVertex?.Point;

				if (source is null)
				{
					Debug.WriteLine("Edge {0}, has no source!", "Error", edge);
					continue;
				}

				Face? face = null;
				int direction = 1;
				if (edge.IncidentFace is not null)
				{
					face = edge.IncidentFace;
				} else if (twin.IncidentFace is not null)
				{
					face = twin.IncidentFace;
					direction *= -1;
				} else
				{
					Debug.WriteLine("Encountered edge that has no face on either side.", "Error");
					continue;
				}

				var centroid = faceCentroidMap[face];

				var faceColor = (edge.IncidentFace is null) ? emptyFaceColor :
					colors[faceIndexMap[face] % colors.Length];
				var twinFaceColor = (twin.IncidentFace is null) ? emptyFaceColor:
					colors[faceIndexMap[face] % colors.Length];

				DrawSegment(source, target, centroid, direction * epsilon, faceColor, canvas);
				DrawSegment(twin.SourceVertex?.Point ?? new Point(0, 0), twin.TargetVertex.Point,
					centroid, -1 * direction * epsilon, twinFaceColor, canvas);

                // Update drawn set.
                drawn.Add(edge);
				drawn.Add(twin);
			}
		}

		private void DrawSegment(Point source, Point target, Point centroid,
			Decimal epsilon, string color, ICanvas canvas)
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
		/// <param name="epsilon">The shfit scale.</param>
		/// <returns>A point along the line p1 -> p2 scaled by epsilon.</returns>
		private static Point ShiftPointTowardPoint(Point p1, Point p2, Decimal epsilon)
		{
			var dx = p2.X - p1.X;
			var dy = p2.Y - p1.Y;
			if (dx == 0)
				return new Point(p1.X, p1.Y + (Math.Sign(dy) * epsilon));

			if (dy == 0)
				return new Point(p1.X + (Math.Sign(dx) * epsilon));

			return new Point(p1.X + (dx * epsilon), p1.Y + (dy * epsilon));
		}
	}
}

