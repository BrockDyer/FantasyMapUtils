using System;
using System.Diagnostics;
using VoronoiModel.Geometry;

namespace VoronoiModel.PlanarSubdivision
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
		/// The vertex that this half edge is coming out of.
		/// </summary>

		/// <summary>
		/// The vertex that this half edge is coming from.
		/// </summary>
		public Vertex? SourceVertex => Twin?.TargetVertex;

		/// <summary>
		/// The line segment that represents this half edge.
		/// </summary>
		public LineSegment2D? Segment => SourceVertex is not null ? 
			new LineSegment2D(SourceVertex.Point, TargetVertex.Point) : null;

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

		/// <summary>
		/// Construct a half edge from the target point.
		/// </summary>
		/// <param name="target">The target point.</param>
		public HalfEdge(Point2D target)
		{
			var v = new Vertex(target, this);
			TargetVertex = v;
		}

		/// <summary>
		/// Construct a half edge from the target vertex. For internal use only.
		/// Target should be an existing vertex in the DCEL and not a newly
		/// constructed one.
		/// </summary>
		/// <param name="target">The target vertex.</param>
		internal HalfEdge(Vertex target)
		{
			TargetVertex = target;
		}

        /// <summary>
        /// Link this half edge to another.
        /// </summary>
        /// <param name="twin">The twin half edge to link to.</param>
        public virtual void LinkTwin(HalfEdge twin)
		{
			Twin = twin;
			twin.Twin = this;
		}

		/// <summary>
		/// Link this half edge to its next edge.
		/// </summary>
		/// <param name="next">The next half edge that this edge points to.</param>
		public virtual void LinkNext(HalfEdge next)
		{
			Next = next;
			next.Previous = this;
		}

        // ============================ Equality ============================ \\
        // A half edge, h1, is equivalent to another, h2, if they have the same
        // source and the same target vertices.

   //      public override bool Equals(object? obj)
   //      {
			// if (obj is HalfEdge h2)
			// {
			// 	var sourcesMatch = this.SourceVertex?.Equals(h2.SourceVertex) ?? false;
			// 	var targetsMatch = this.TargetVertex.Equals(h2.TargetVertex);
			// 	return sourcesMatch && targetsMatch;
			// }
   //
			// return false;
   //      }
   //
   //      public override int GetHashCode()
   //      {
			// var hash = 17;
			// hash = hash * 33 + (SourceVertex?.GetHashCode() ?? 17);
			// hash = hash * 33 + TargetVertex.GetHashCode();
			// return hash;
   //      }

        // ================================================================== \\

        public override string ToString()
        {
			return $"{SourceVertex} -> {TargetVertex}";
        }
    }
}

