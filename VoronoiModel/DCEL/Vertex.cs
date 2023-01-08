using System;
using VoronoiModel.Geometry;

namespace VoronoiModel.DCEL
{
    /// <summary>
    /// A vertex in the DCEL. Has a point location and an edge. The edge is an
    /// edge that has this vertex as its target.
    /// </summary>
    public class Vertex
    {
        public Vector Point { get; internal set; }
        public HalfEdge? Edge { get; internal set; }

        public Vertex(Decimal x1, Decimal x2)
        {
            Point = new Vector(x1, x2);
        }

        /// <summary>
        /// Get all of the edges that are incident to this vertex.
        /// </summary>
        /// <returns>A list of edges that are incident to this vertex.</returns>
        public List<HalfEdge> GetIncidentEdges()
        {
            var startEdge = Edge;
            var edges = new List<HalfEdge>();
            var current = startEdge;

            throw new NotImplementedException();

            //return edges;
        }

        // ============================ Equality ============================ \\
        // A vertex is equal to another if they have the same point.

        public override bool Equals(object? obj)
        {
            return obj is Vertex vertex &&
                   EqualityComparer<Vector>.Default.Equals(Point, vertex.Point);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Point);
        }

        // ================================================================== \\
    }
}

