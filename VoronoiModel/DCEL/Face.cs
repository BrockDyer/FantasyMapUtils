using System;
using VoronoiModel.Geometry;

namespace VoronoiModel.DCEL
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
		public HalfEdge? Edge { get; internal set; }

		/// <summary>
		/// Check if a given point vector is contained within this face.
		/// </summary>
		/// <param name="point">The point vector to check.</param>
		/// <returns>True if the point vector is contained within the face.
		/// False otherwise.</returns>
		public bool ContainsPoint(Vector point)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the edges that compose this face.
		/// </summary>
		/// <returns>A list of half edges that are incident to this face.</returns>
		public List<HalfEdge> GetFaceEdges()
		{
			throw new NotImplementedException();
		}

        // =========================== Equality ============================= \\
        // A face, f1, is equivalent to another face, f2, if they have the same
        // edges.

        public override bool Equals(object? obj)
        {
			if (obj is Face f2)
			{

				if (Edge is null)
				{
					return f2.Edge is null;
				}

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
			return HashCode.Combine(GetFaceEdges());
        }

        // ================================================================== \\
    }
}

