using System;
using Microsoft.Maui.Graphics;
using VoronoiModel.Geometry;

namespace VoronoiModel.DCEL
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
		HashSet<HalfEdge> HalfEdges { get; } = new();
		HashSet<Vertex> Vertices { get; } = new();
		HashSet<Face> Faces { get; } = new();

		/// <summary>
		/// Initialize the DCEL to a planar region.
		/// </summary>
		/// <param name="x1">The x coordinate of the upper left point.</param>
		/// <param name="y1">The y coordinate of the upper left point.</param>
		/// <param name="x2">The x coordinate of the lower right point.</param>
		/// <param name="y2">The y coordinate of the lower right point.</param>
		public DCEL(Decimal x1, Decimal y1, Decimal x2, Decimal y2)
		{
			// Construct vertices.
			var vul = new Vertex(x1, y1);
			var vur = new Vertex(x2, y1);
			var vlr = new Vertex(x2, y2);
			var vll = new Vertex(x1, y2);

			// Construct half edges
			var halfEdges = new List<HalfEdge>();
			foreach (var vertex in new Vertex[]{ vul, vur, vlr, vll, vll, vul, vur, vlr })
			{
				var edge = new HalfEdge(vertex);
				halfEdges.Add(edge);
				vertex.Edge = edge;
			}

			// Construct face
			var face = new Face();
			face.Edge = halfEdges[0];

			// Post-construction initialization of half edges.
			for (var i = 0; i < 4; i++)
			{
				var h1 = halfEdges[i];
				var h2 = halfEdges[i + 4];

				// Set twins
				h1.Twin = h2;
				h2.Twin = h1;

				// Set line segments
				h1.Segment = new LineSegment(h1.GetSource().Point, h1.TargetVertex.Point);
				h2.Segment = new LineSegment(h2.GetSource().Point, h2.TargetVertex.Point);

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

			Faces.Add(face);
			HalfEdges.UnionWith(halfEdges);
			Vertices.UnionWith(new Vertex[] {vul, vur, vlr, vll});;
		}

		/// <summary>
		/// Initialie the DCEL to a planar region defined by two point vectors.
		/// </summary>
		/// <param name="v1">The vector represnting the upper left point.</param>
		/// <param name="v2">The vector representing the lower right point.</param>
		public DCEL(Vector v1, Vector v2) : this(v1.GetX(), v1.GetY(), v2.GetX(), v2.GetZ()) { }

		/// <summary>
		/// Add a vertex at point v1 that is connected to the vertex at point v2.
		/// v2 must be an existing vertex.
		/// </summary>
		/// <param name="v1">The new vertex to add as a point vector.</param>
		/// <param name="v2">The existing vertex to connect v1 to as a point vector.</param>
		public void AddVertex(Vector v1, Vector v2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Split a given half edge by adding a new vertex.
		/// </summary>
		/// <param name="v">The vertex to add as a point vector.</param>
		/// <param name="h">The half edge to split.</param>
		public void SplitEdge(Vector v, HalfEdge h)
		{
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
		public Tuple<Face, Face> SplitFace(Vector v, Vector u)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete a given half edge, thus merging two faces. Returns the newly
		/// merged face.
		/// </summary>
		/// <param name="h">The half edge to delete.</param>
		/// <returns>The new face.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public Face DeleteEdge(HalfEdge h)
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
		public void DeleteVertex(Vector v)
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
			throw new NotImplementedException();
		}
	}
}

