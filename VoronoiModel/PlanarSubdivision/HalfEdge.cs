﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
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
		public Vertex? SourceVertex { get; internal set; }

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
		}

        /// <summary>
        /// Link this half edge to another.
        /// </summary>
        /// <param name="twin">The twin half edge to link to.</param>
        public virtual void LinkTwin(HalfEdge twin)
		{
			Twin = twin;
			SourceVertex = twin.TargetVertex;
			twin.Twin = this;
			twin.SourceVertex = TargetVertex;
		}

		/// <summary>
		/// Link this half edge to its next edge.
		/// </summary>
		/// <param name="next">The next half edge that this edge points to.</param>
		public virtual void LinkNext(HalfEdge next)
		{
			Next = next;
			next.Previous = this;
			next.SourceVertex = TargetVertex;
		}

        // ============================ Equality ============================ \\
        // A half edge, h1, is equivalent to another, h2, if they have the same
        // source and the same target vertices.

        public override bool Equals(object? obj)
        {
			if (obj is HalfEdge h2)
			{
				var sourcesMatch = this.SourceVertex?.Equals(h2.SourceVertex) ?? false;
				var targetsMatch = this.TargetVertex.Equals(h2.TargetVertex);
				return sourcesMatch && targetsMatch;
			}

			return false;
        }

        public override int GetHashCode()
        {
			var hash = 17;
			hash = hash * 33 + (SourceVertex?.GetHashCode() ?? 17);
			hash = hash * 33 + TargetVertex.GetHashCode();
			return hash;
        }

        // ================================================================== \\

        public override string ToString()
        {
			return string.Format("{0} -> {1}", SourceVertex, TargetVertex);
        }
    }
}

