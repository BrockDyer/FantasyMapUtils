using System;
using VoronoiModel.Geometry;

namespace VoronoiModel.DCEL
{
	/// <summary>
	/// One half of an edge between two vertices in the DCEL. Separating edges
	/// into half edges allows efficient navigation of faces in the DCEL.
	/// </summary>
	public class HalfEdge
	{
		/// <summary>
		/// The vertex that this half edge is pointing to.
		/// </summary>
        public Vertex TargetVertex { get; internal set; }
		/// <summary>
		/// The actual segment that represents this half edge.
		/// </summary>
		public ISegment? Segment { get; internal set; }
		/// <summary>
		/// The twin of this half edge.
		/// </summary>
		public HalfEdge? Twin { get; internal set; }
		/// <summary>
		/// The next half edge on this incident face.
		/// </summary>
		public HalfEdge? Next { get; internal set; }
		/// <summary>
		/// The previous half edge on this incident face.
		/// </summary>
		public HalfEdge? Previous { get; internal set; }
		/// <summary>
		/// The face that is incident with this half edge.
		/// </summary>
		public Face? IncidentFace { get; internal set; }

        public HalfEdge(Vertex target)
		{
			TargetVertex = target;
			Segment = null;
			Twin = null;
			Next = null;
			Previous = null;
			IncidentFace = null;
		}

		/// <summary>
		/// Get the source vertex of this half edge.
		/// </summary>
		/// <returns>The source <see cref="Vertex"/>.</returns>
		/// <exception cref="InvalidOperationException"/>
		public Vertex GetSource()
		{
			if (Twin is null) throw new InvalidOperationException("Cannot get source because it is null");
			return this.Twin.TargetVertex;
		}

        // ============================ Equality ============================ \\
        // A half edge, h1, is equivalent to another, h2, if they have the same
        // source and the same target vertices.

        public override bool Equals(object? obj)
        {
			if (obj is HalfEdge h2)
			{
				var sourcesMatch = this.GetSource()?.Equals(h2.GetSource());
				var targetsMatch = this.TargetVertex.Equals(h2.TargetVertex);
				return sourcesMatch.GetValueOrDefault(false) && targetsMatch;
			}

			return false;
        }

        public override int GetHashCode()
        {
			return HashCode.Combine(this.TargetVertex, this.GetSource());
        }

        // ================================================================== \\
    }
}

