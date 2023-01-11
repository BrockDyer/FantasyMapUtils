using System;
using System.Diagnostics;
using VoronoiModel.Geometry;

namespace VoronoiModel.PlanarSubdivision
{
    /// <summary>
    /// A vertex in the DCEL. Has a point location and an edge. The edge is an
    /// edge that has this vertex as its target.
    /// </summary>
    public class Vertex
    {
        public Point2D Point { get; internal set; }
        public HalfEdge Edge { get; internal set; }

        public Vertex(double x1, double x2, HalfEdge edge) : this(new Point2D(x1, x2), edge) { }

        internal Vertex(Point2D p, HalfEdge edge)
        {
            Point = p;
            Edge = edge;
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

            return edges;
        }

        // ============================ Equality ============================ \\
        // A vertex is equal to another if they have the same point.

        public override bool Equals(object? obj)
        {
            return obj is Vertex vertex &&
                   EqualityComparer<Point2D>.Default.Equals(Point, vertex.Point);
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }

        // ================================================================== \\

        public override string ToString()
        {
            return Point.ToString();
        }
    }
}

