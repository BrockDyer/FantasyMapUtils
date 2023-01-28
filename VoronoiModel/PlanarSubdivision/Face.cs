using System.Diagnostics;
using VoronoiModel.Geometry;

namespace VoronoiModel.PlanarSubdivision
{
	/// <summary>
	/// A face contains a reference to the half edge that is incident to it.
	/// This edge can be followed clockwise (via Next) to traverse this face.
	/// Thus a face is defined by all of its connected clockwise half edges.
	/// </summary>
	public class Face
	{
		/// <summary>
		/// A half edge incident to this face.
		/// </summary>
		private HalfEdge Edge { get; }

		public Face(HalfEdge edge)
		{
			Edge = edge;
		}

		/// <summary>
		/// Update the IncidentEdge field on all edges on this face.
		/// </summary>
		public void LinkEdges()
		{
			foreach (var edge in GetFaceEdges())
			{
				edge.IncidentFace = this;
			}
		}

		/// <summary>
		/// Check if this face is an external face. If the edges proceed in a clockwise manner, it is an
		/// external face.
		/// </summary>
		/// <returns>True if this face is external.</returns>
		public bool IsFaceExternal()
		{
			var lag = Edge;
			var next = Edge.Next!;
			while (!next.Equals(Edge))
			{
				var a = lag.SourceVertex!.Point;
				var b = lag.TargetVertex.Point;
				var c = next.TargetVertex.Point;

				var u = new Vector(b.X - a.X, b.Y - a.Y);
				var v = new Vector(c.X - b.X, c.Y - b.Y);

				var crossProduct = u.Cross2D(v);
				var z = crossProduct.Get(2);
				switch (z)
				{
					case < 0:
						return false;
					case > 0:
						return true;
					default:
						lag = next;
						next = lag.Next!;
						break;
				}
			}

			return false;
		}

		/// <summary>
		/// Check if a given point vector is contained within this face. Using the logic that a point is within a
		/// polygon if a horizontal ray shot from the point intersects with an odd number of edges. A point is also
		/// inside the face if it is on a boundary segment of the face.
		/// </summary>
		/// <param name="point">The point vector to check.</param>
		/// <returns>True if the point is contained within the face.
		/// False otherwise.</returns>
		public bool ContainsPoint(Point2D point)
		{
			var intersections = 0;
			foreach (var edge in GetFaceEdges())
			{
				// Consider a point on the face boundary to be inside the edge.
				if (edge.Segment?.IsPointOn(point) ?? false) return true;
				if (edge.Segment?.IntersectsWithLeftRayFrom(point) ?? false)
				{
					intersections += 1;
				}
			}

			return intersections % 2 == 1;
		}

		/// <summary>
		/// Get the edges that compose this face.
		/// </summary>
		/// <returns>A list of half edges that are incident to this face.</returns>
		public List<HalfEdge> GetFaceEdges()
		{
			var edges = new List<HalfEdge> { Edge };

			var current = Edge.Next;
			while (!Edge.Equals(current) && current is not null)
			{
				edges.Add(current);
				current = current.Next;
			}

			return edges;
		}

        /// <summary>
        /// Compute the centroid of the face.
		/// <see href="https://en.wikipedia.org/wiki/Centroid#Centroid_of_polygon">Formula from Wikipedia</see>
        /// </summary>
        /// <returns>The point vector of the centroid.</returns>
		/// <exception cref="InvalidOperationException"/>
        public Point2D Centroid()
		{
			var vertices = new List<Point2D>();
			var start = Edge;
			var current = Edge;
			while (true)
			{
				if (current is null)
				{
					const string msg = "Face is not defined by complete edges.";
					Debug.WriteLine(msg);
					throw new InvalidOperationException(msg);
				}

				vertices.Add(current.TargetVertex.Point);
				current = current.Next;
				if (!EqualityComparer<HalfEdge>.Default.Equals(current, start)) continue;
				
				// Need to have the start vertex in twice.
				vertices.Add(current.TargetVertex.Point);
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

        // =========================== Equality ============================= \\
        // A face, f1, is equivalent to another face, f2, if they have the same
        // edges.

   //      public override bool Equals(object? obj)
   //      {
			// //Debug.WriteLine("Calling equals on Face");
			// if (obj is Face f2)
			// {
   //
			// 	HalfEdge? start = null;
			// 	HalfEdge? currentF1 = null;
			// 	HalfEdge? currentF2 = null;
   //
			// 	// Find an edge that the faces share.
			// 	foreach (var edge in f2.GetFaceEdges())
			// 	{
			// 		if (!Edge.Equals(edge)) continue;
			// 		
			// 		start = Edge;
			// 		currentF1 = Edge;
			// 		currentF2 = edge;
			// 		break;
			// 	}
   //
			// 	// If they do not share an edge they are not equal
			// 	if (start is null) return false;
   //
			// 	// Walk through the edges on each face. They should be lockstep
			// 	while (true)
			// 	{
			// 		if (!EqualityComparer<HalfEdge>.Default.Equals(currentF1, currentF2))
			// 			return false;
   //
			// 		currentF1 = currentF1?.Next;
			// 		currentF2 = currentF2?.Next;
   //
			// 		if (EqualityComparer<HalfEdge>.Default.Equals(currentF1, start))
			// 			break;
   //
			// 	}
   //
			// 	// If we get to this point then the faces are equal.
			// 	return true;
			// }
			// return false;
   //      }
   //
   //      public override int GetHashCode()
   //      {
			// //Debug.WriteLine("Calling hashcode on Face");
			// var hash = 17;
			// foreach(var edge in GetFaceEdges())
			// {
			// 	hash += edge.GetHashCode();
			// }
			// hash *= 33;
			// return hash;
   //      }

        // ================================================================== \\

        public override string ToString()
        {
			var output = "";
			var prefix = "";
			foreach (var edge in GetFaceEdges())
			{
				output += prefix + edge;
				prefix = "; ";
			}

			return output;
        }
    }
}

