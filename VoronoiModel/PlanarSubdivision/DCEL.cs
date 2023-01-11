using System;
using System.Diagnostics;
using Microsoft.Maui.Graphics;
using VoronoiModel.Geometry;

namespace VoronoiModel.PlanarSubdivision
{
	/// <summary>
	/// An exception that is raised when attempting to split a half edge by
	/// a new vertex and the vertex does not fall on that half edge.
	/// </summary>
	public class PointNotOnHalfEdgeException : Exception
	{
		public PointNotOnHalfEdgeException(string msg) : base(msg) { }
	}

	/// <summary>
	/// An exception that is raised when attempting to perform an operation with
	/// an existing vertex u, but u does not exist.
	/// </summary>
	public class VertexNotFoundException : Exception
	{
		public VertexNotFoundException(string msg) : base(msg) { }
	}

	/// <summary>
	/// An exception that should be raised when the DCEL is detected to be in a
	/// corrupted state.
	/// </summary>
	public class CorruptedDCELException : Exception
	{
		public CorruptedDCELException(string msg) : base(msg) { }
	}

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
		Dictionary<Point2D, Dictionary<Point2D, HalfEdge>> HalfEdges { get; } = new();
		Dictionary<Point2D, Vertex> Vertices { get; } = new();
        HashSet<Face> Faces { get; } = new();

		/// <summary>
		/// Get a list of the faces in this DCEL.
		/// </summary>
		/// <returns>A list of faces.</returns>
		public List<Face> GetFaces()
		{
			return Faces.ToList();
		}

		/// <summary>
		/// Get the face that is incident to a given half edge.
		/// </summary>
		/// <param name="source">The source of the half edge.</param>
		/// <param name="target">The target of the half edge.</param>
		/// <returns></returns>
		public Face? GetFaceWithEdge(Point2D source, Point2D target)
		{
			return GetEdge(source, target)?.IncidentFace;
		}

		/// <summary>
		/// Create a new instance of the DCEL object.
		/// </summary>
		private DCEL() {}

		/// <summary>
		/// Get a half edge by its source and target vertex.
		/// </summary>
		/// <param name="source">The source point.</param>
		/// <param name="target">The target point.</param>
		/// <returns>The half edge, if it exists.</returns>
		private HalfEdge? GetEdge(Point2D source, Point2D target)
		{
			HalfEdge? edge = null;
			HalfEdges.TryGetValue(source, out var map);
			map?.TryGetValue(target, out edge);
			return edge;
		}

		/// <summary>
		/// Helper method to add a complete half edge to the map. It must have
		/// both its target and source set.
		/// </summary>
		/// <param name="edge">The half edge to add.</param>
		/// <exception cref="CorruptedDCELException">If the half edge is not complete.</exception>
		private void AddEdgeToMap(HalfEdge edge)
		{
			var source = edge.SourceVertex?.Point;
			var target = edge.TargetVertex.Point;

			if (source is null)
			{
				var msg = string.Format("Attempted to add an incomplete half edge {0} to the DCEL.", edge);
				Debug.WriteLine(msg, "Error");
				throw new CorruptedDCELException(msg);
			}

			if (!HalfEdges.ContainsKey(source))
			{
				HalfEdges.Add(source, new());
			}

			HalfEdges[source].Add(target, edge);
		}

		/// <summary>
		/// Get a vertex from the map if it exists.
		/// </summary>
		/// <param name="p">The point location of the vertex.</param>
		/// <returns>The vertex at the given point.</returns>
		private Vertex? GetVertex(Point2D p)
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
		public static DCEL Create(Point2D upperLeft, Point2D lowerRight)
		{
			var dcel = new DCEL();

            // Construct vertices.
            var vul = new Point2D(upperLeft.X, upperLeft.Y);
            var vur = new Point2D(lowerRight.X, upperLeft.Y);
            var vlr = new Point2D(lowerRight.X, lowerRight.Y);
            var vll = new Point2D(upperLeft.X, lowerRight.Y);

            // Construct half edges
            var halfEdges = new List<HalfEdge>();
            foreach (var vertexPoint in new Point2D[] { vul, vur, vlr, vll, vll, vul, vur, vlr })
            {
                var edge = new HalfEdge(vertexPoint);
                halfEdges.Add(edge);
            }

            // Construct face
            var face = new Face(halfEdges[0]);

            // Post-construction initialization of half edges.
            for (var i = 0; i < 4; i++)
            {
                var h1 = halfEdges[i];
                var h2 = halfEdges[i + 4];

				// Set twins
				//h1.LinkTwin(h2)

				// Manually initializing so that Intellisense and Compiler
				// recognize that source vertices are not null.
				h1.Twin = h2;
				h2.Twin = h1;
				h1.SourceVertex = h2.TargetVertex;
				h2.SourceVertex = h1.TargetVertex;

                // Set line segments
                h1.Segment = new LineSegment2D(h1.SourceVertex.Point, h1.TargetVertex.Point);
                h2.Segment = new LineSegment2D(h2.SourceVertex.Point, h2.TargetVertex.Point);

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

			//foreach(var pointVertex in new Point[] {vul, vur, vlr, vll })
			//{
			//	dcel.Vertices.Add(pointVertex.Point, pointVertex);
			//}
			for (var i = 0; i < halfEdges.Count / 2; i++)
			{
				var edge = halfEdges[i];
				var vertex = edge.TargetVertex;
				var point = vertex.Point;
				dcel.Vertices.Add(point, vertex);
			}

			return dcel;
        }

		/// <summary>
		/// Add a vertex at point v1 that is connected to the vertex at point v2.
		/// v2 must be an existing vertex.
		/// </summary>
		/// <param name="v1">The new vertex to add as a point vector.</param>
		/// <param name="v2">The existing vertex to connect v1 to as a point vector.</param>
		public void AddVertex(Point2D v1, Point2D v2)
		{
			var existingVertex = GetVertex(v2);
			if (existingVertex is null)
			{
				var msg = string.Format("Could not find vertex {0} in the DCEL.", v2);
				Debug.WriteLine(msg, "Warn");
				throw new VertexNotFoundException(msg);
			}

			// Find a valid half edge.
			HalfEdge? h1 = null;
			foreach (var edge in existingVertex.GetIncidentEdges())
			{
				if (!edge.TargetVertex.Equals(existingVertex) || edge.IncidentFace is null)
					continue;

				if (edge.IncidentFace.ContainsPoint(v1))
				{
					h1 = edge;
					break;
				}
			}

			if (h1 is null)
			{
				var msg = string.Format("Could not find an edge connected to {0} with a " +
					"face that contains {1}", v2, v1);
				Debug.WriteLine(msg, "Warn");
				throw new InvalidOperationException(msg);
			}

			var h2 = h1.Next;
			if (h2 is null)
			{
				var msg = string.Format("HalfEdge {0} has no next edge in DCEL. This is bad!", h1);
				Debug.WriteLine(msg, "Error");
				throw new CorruptedDCELException(msg);
			}

			var h3 = new HalfEdge(v1);
			var h3twin = new HalfEdge(existingVertex);

            // Update prev, next, and twins
            h1.LinkNext(h3);
            h3.LinkNext(h3twin);
			h3twin.LinkNext(h2);
			h3.LinkTwin(h3twin);

			// Update incident faces
			h3.IncidentFace = h1.IncidentFace;
			h3twin.IncidentFace = h2.IncidentFace;

			// Update DCEL fields.
			AddEdgeToMap(h3);
			AddEdgeToMap(h3twin);
			Vertices.Add(v1, h3.TargetVertex);
		}

        /// <summary>
        /// Split a given half edge by adding a new vertex.
        /// </summary>
        /// <param name="v">The vertex to add as a point vector.</param>
        /// <param name="source">The source vertex of the half edge to split.</param>
        /// <param name="target">The target vertex of the half edge to split.</param>
        public void SplitEdge(Point2D v, Point2D source, Point2D target)
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
		public Tuple<Face, Face> SplitFace(Point2D v, Point2D u)
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
		public Face DeleteEdge(Point2D source, Point2D target)
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

            var epsilon = 0.01d;
            Dictionary<Face, int> faceIndexMap = new();
            Dictionary<Face, Point2D> faceCentroidMap = new();

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
				DrawSegment(twin.SourceVertex?.Point ?? new Point2D(0, 0), twin.TargetVertex.Point,
					centroid, -1 * direction * epsilon, twinFaceColor, canvas);

                // Update drawn set.
                drawn.Add(edge);
				drawn.Add(twin);
			}
		}

		private void DrawSegment(Point2D source, Point2D target, Point2D centroid,
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
		/// <param name="epsilon">The shfit scale.</param>
		/// <returns>A point along the line p1 -> p2 scaled by epsilon.</returns>
		private static Point2D ShiftPointTowardPoint(Point2D p1, Point2D p2, double epsilon)
		{
			var dx = p2.X - p1.X;
			var dy = p2.Y - p1.Y;
			if (dx == 0)
				return new Point2D(p1.X, p1.Y + (Math.Sign(dy) * epsilon));

			if (dy == 0)
				return new Point2D(p1.X + (Math.Sign(dx) * epsilon), p1.Y);

			return new Point2D(p1.X + (dx * epsilon), p1.Y + (dy * epsilon));
		}
	}
}

