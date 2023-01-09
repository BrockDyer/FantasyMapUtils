using System;
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
		public HalfEdge Edge { get; internal set; }

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
		/// Check if a given point vector is contained within this face.
		/// </summary>
		/// <param name="point">The point vector to check.</param>
		/// <returns>True if the point vector is contained within the face.
		/// False otherwise.</returns>
		public bool ContainsPoint(Point point)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the edges that compose this face.
		/// </summary>
		/// <returns>A list of half edges that are incident to this face.</returns>
		public List<HalfEdge> GetFaceEdges()
		{
			var edges = new List<HalfEdge>();
			edges.Add(Edge);

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
        public Point Centroid()
		{
			var vertices = new List<Point>();
			var start = Edge;
			var current = Edge;
			while (true)
			{
				if (current is null)
				{
					var msg = "Face is not defined by complete edges.";
					Debug.WriteLine(msg);
					throw new InvalidOperationException(msg);
				}

				vertices.Add(current.TargetVertex.Point);
				current = current.Next;
				if (EqualityComparer<HalfEdge>.Default.Equals(current, start))
				{
					// Need to have the start vertex in twice.
					vertices.Add(current.TargetVertex.Point);
					break;
				}
			}

			var A = SignedArea(vertices);
			var CxSum = 0M;
			var CySum = 0M;
			for (var i = 0; i < vertices.Count - 1; i++)
			{
				var vi = vertices[i];
				var vn = vertices[i + 1];
				CxSum += (vi.X + vn.X) * ((vi.X * vn.Y) - (vn.X * vi.Y));
				CySum += (vi.Y + vn.Y) * ((vi.X * vn.Y) - (vn.X * vi.Y));
			}

			var Cx = (1M / 6M * A) * CxSum;
			var Cy = (1M / 6M * A) * CySum;

			return new Point(Math.Round(Cx, 3), Math.Round(Cy, 3));
		}

		/// <summary>
		/// Compute the signed area of the polygon as calculated by the
		/// shoelace formula.
		/// </summary>
		/// <param name="vertices">A list of the point vector vertices. Must
		/// have the start point duplicated at the end.</param>
		/// <returns>The signed area.</returns>
		private Decimal SignedArea(List<Point> vertices)
		{
			var sum = 0M;
			for (var i = 0; i < vertices.Count - 1; i++)
			{
				var vi = vertices[i];
				var vn = vertices[i + 1];

				sum += (vi.X * vn.Y) - (vn.X * vi.Y);
			}
			return 0.5M * sum;
		}

        // =========================== Equality ============================= \\
        // A face, f1, is equivalent to another face, f2, if they have the same
        // edges.

        public override bool Equals(object? obj)
        {
			//Debug.WriteLine("Calling equals on Face");
			if (obj is Face f2)
			{

				HalfEdge? start = null;
				HalfEdge? current_f1 = null;
				HalfEdge? current_f2 = null;

				// Find an edge that the faces share.
				foreach (var edge in f2.GetFaceEdges())
				{
					if (Edge.Equals(edge))
					{
						start = Edge;
						current_f1 = Edge;
						current_f2 = edge;
						break;
					}
				}

				// If they do not share an edge they are not equal
				if (start is null) return false;

				// Walk through the edges on each face. They should be lockstep
				while (true)
				{
					if (!EqualityComparer<HalfEdge>.Default.Equals(current_f1, current_f2))
						return false;

					current_f1 = current_f1?.Next;
					current_f2 = current_f2?.Next;

					if (EqualityComparer<HalfEdge>.Default.Equals(current_f1, start))
						break;

				}

				// If we get to this point then the faces are equal.
				return true;
			}
			return false;
        }

        public override int GetHashCode()
        {
			//Debug.WriteLine("Calling hashcode on Face");
			var hash = 17;
			foreach(var edge in GetFaceEdges())
			{
				hash += edge.GetHashCode();
			}
			hash *= 33;
			return hash;
        }

        // ================================================================== \\

        public override string ToString()
        {
			var output = "";
			var prefix = "";
			foreach (var edge in GetFaceEdges())
			{
				output += prefix + edge.ToString();
				prefix = "; ";
			}

			return output;
        }
    }
}

