﻿using System.Diagnostics;
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
	/// An exception that is raised when attempting to perform an operation with an existing half edge h, but h does
	/// not exist.
	/// </summary>
	public class HalfEdgeNotFoundException : Exception
	{
		public HalfEdgeNotFoundException(string msg) : base(msg) { }
	}

	/// <summary>
	/// An exception that should be raised when the DCEL is detected to be in a
	/// corrupted state.
	/// </summary>
	public class CorruptedDcelException : Exception
	{
		public CorruptedDcelException(string msg) : base(msg) { }
	}

	/// <summary>
	/// A subdivision of a planar space into faces. Faces may be traversed by
	/// following the next and previous properties of any edge incident to that
	/// face. Adjacent faces can be navigated to via the twin of the corresponding
	/// half edge on the current face.
	/// </summary>
	public class Dcel
	{
		//HashSet<HalfEdge> HalfEdges { get; } = new();
		//HashSet<Vertex> Vertices { get; } = new();

		/// <summary>
		/// Efficiently store half edges as Source -> Target -> HalfEdge
		/// </summary>
		private Dictionary<Point2D, Dictionary<Point2D, HalfEdge>> HalfEdges { get; } = new();

		private Dictionary<Point2D, Vertex> Vertices { get; } = new();
		private List<Face> Faces { get; } = new();

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
		private Dcel() {}

		private Dcel(Dictionary<Point2D, Vertex> vertices, Dictionary<Point2D, Dictionary<Point2D, HalfEdge>> halfEdges,
			List<Face> faces)
		{
			Vertices = vertices;
			HalfEdges = halfEdges;
			Faces = faces;
		}

		/// <summary>
		/// Get a half edge by its source and target vertex.
		/// </summary>
		/// <param name="source">The source point.</param>
		/// <param name="target">The target point.</param>
		/// <returns>The half edge, if it exists.</returns>
		public HalfEdge? GetEdge(Point2D source, Point2D target)
		{
			HalfEdge? edge = null;
			HalfEdges.TryGetValue(source, out var map);
			map?.TryGetValue(target, out edge);
			return edge;
		}

		/// <summary>
		/// Use this method to update the target of a vertex. This will update the map accordingly. When the edge changes
		/// target, the source of its twin changes. Thus we also need to update the twin edge in the map.
		/// </summary>
		/// <param name="edge">The edge to update.</param>
		/// <param name="target">The vertex it is now targeting.</param>
		/// <exception cref="CorruptedDcelException">If the edge does not have a source vertex.</exception>
		private void UpdateEdgeTarget(HalfEdge edge, Vertex target)
		{
			var oldTarget = edge.TargetVertex.Point;
			var newTarget = target.Point;
			
			var source = edge.SourceVertex?.Point;
			if (source is null) throw new CorruptedDcelException($"Half edge {edge} has no source.");

			var twin = edge.Twin;
			if (twin is null) throw new CorruptedDcelException($"Half edge {edge} has no twin");
			var twinTarget = twin.TargetVertex.Point;
			var twinSource = twin.SourceVertex?.Point;
			if (twinSource is null) throw new CorruptedDcelException($"Half edge {edge.Twin} has no source.");

			// Update map for this edge.
			HalfEdges[source].Remove(oldTarget);
			HalfEdges[source].Add(newTarget, edge);

			HalfEdges[twinSource].Remove(twinTarget);
			if (!HalfEdges.ContainsKey(newTarget))
				HalfEdges.Add(newTarget, new Dictionary<Point2D, HalfEdge>());
			// The new target becomes the source of the twin, and it keeps its current target.
			HalfEdges[newTarget].Add(twinTarget, twin);
			
			// Update the target
			edge.TargetVertex = target;
		}

		/// <summary>
		/// Helper method to add a complete half edge to the map. It must have
		/// both its target and source set.
		/// </summary>
		/// <param name="edge">The half edge to add.</param>
		/// <exception cref="CorruptedDcelException">If the half edge is not complete.</exception>
		private void AddEdgeToMap(HalfEdge edge)
		{
			var source = edge.SourceVertex?.Point;
			var target = edge.TargetVertex.Point;

			if (source is null)
			{
				var msg = $"Attempted to add an incomplete half edge {edge} to the DCEL.";
				Debug.WriteLine(msg, "Error");
				throw new CorruptedDcelException(msg);
			}

			if (!HalfEdges.ContainsKey(source))
			{
				HalfEdges.Add(source, new Dictionary<Point2D, HalfEdge>());
			}

			HalfEdges[source].Add(target, edge);
		}

		/// <summary>
		/// Get a vertex from the map if it exists.
		/// </summary>
		/// <param name="p">The point location of the vertex.</param>
		/// <returns>The vertex at the given point.</returns>
		private Vertex GetVertex(Point2D p)
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
		public static Dcel Create(Point2D upperLeft, Point2D lowerRight)
		{
			var dcel = new Dcel();

			// Construct vertices.
			var vul = new Point2D(upperLeft.X, upperLeft.Y);
			var vur = new Point2D(lowerRight.X, upperLeft.Y);
			var vlr = new Point2D(lowerRight.X, lowerRight.Y);
			var vll = new Point2D(upperLeft.X, lowerRight.Y);

			var points = new[] { vul, vur, vlr, vll };
			var vertices = new List<Vertex>();

			// Create half edges
			var interiorEdges = new List<HalfEdge>();
			var exteriorEdges = new List<HalfEdge>(points.Select<Point2D, HalfEdge>(_ => null!));
			
			for (var i = 0; i < points.Length; i++)
			{
				var point = points[i];
				var interiorEdge = new HalfEdge(point);
				var vertex = interiorEdge.TargetVertex;
				var nextInteriorTwin = new HalfEdge(vertex);
				
				vertices.Add(vertex);
				interiorEdges.Add(interiorEdge);

				var exteriorIndex = (i + 1) % points.Length;
				exteriorEdges[exteriorIndex] = nextInteriorTwin;
			}
			
			// Link half edges
			for (var i = 0; i < points.Length; i++)
			{
				var edge = interiorEdges[i];
				var twin = exteriorEdges[i];

				var linkIndex = (i + 1) % points.Length;
				
				var next = interiorEdges[linkIndex];
				edge.LinkNext(next);

				var twinPrev = exteriorEdges[linkIndex];
				twinPrev.LinkNext(twin);
				
				edge.LinkTwin(twin);
			}
			
			// Create faces
			var face = new Face(interiorEdges[0]);
			face.LinkEdges();
			
			// Update dcel state storage
			
			// Add vertices
			foreach (var vertex in vertices)
			{
				dcel.Vertices.Add(vertex.Point, vertex);
			}
			
			// Add edges
			foreach (var edge in interiorEdges.Union(exteriorEdges))
			{
				dcel.AddEdgeToMap(edge);
			}
			
			// Add face
			dcel.Faces.Add(face);

			return dcel;
        }

		/// <summary>
		/// Create a doubly connected edge list to represent a planar subdivision of the plane given by the upper left
		/// point and the lower right point. Constructs the DCEL using the provided segments.
		/// </summary>
		/// <param name="upperLeft">The upper left point of the bounding box.</param>
		/// <param name="lowerRight">The lower right point of the bounding box.</param>
		/// <param name="segments">The segments of the DCEL (not including bounding box segments).</param>
		/// <returns>The constructed DCEL.</returns>
		public static Dcel Create(Point2D upperLeft, Point2D lowerRight, List<LineSegment2D> segments)
		{
			// var vertices = new Dictionary<Point2D, Vertex>();
			var edgesOnVertex = new Dictionary<Point2D, HashSet<LineSegment2D>>();
			var halfEdges = new Dictionary<Point2D, Dictionary<Point2D, HalfEdge>>();
			var faces = new List<Face>();

			var upperRight = new Point2D(lowerRight.X, upperLeft.Y);
			var lowerLeft = new Point2D(upperLeft.X, lowerRight.Y);
			
			var boundingBox = new[]
			{
				new LineSegment2D(upperLeft, upperRight),
				new LineSegment2D(upperRight, lowerRight),
				new LineSegment2D(lowerRight, lowerLeft),
				new LineSegment2D(lowerLeft, upperLeft)
			};

			bool IsPointOnBoundingBox(Point2D point)
			{
				return boundingBox.Any(line => line.IsPointOn(point));
			}
			
			// Establish the line segments that correspond to each vertex.
			foreach (var (segment, point) in segments.SelectMany(segment => 
				         new[] {Tuple.Create(segment, segment.Start), Tuple.Create(segment, segment.End) }))
			{
				// We will handle edges passing straight through later.
				if (IsPointOnBoundingBox(point)) continue;
				
				if (!edgesOnVertex.ContainsKey(point))
				{
					edgesOnVertex[point] = new HashSet<LineSegment2D>();
				}
				
				// Convert segment so that it points out from the vertex.
				var end = segment.Start.Equals(point) ? segment.End : segment.Start;
				var segmentToUse = new LineSegment2D(point, end);
				edgesOnVertex[point].Add(segmentToUse);
			}

			var segmentsStraightThrough = segments.Where(s =>
				IsPointOnBoundingBox(s.Start) && IsPointOnBoundingBox(s.End));
			
			// Handle edges passing straight through.
			foreach (var segment in segmentsStraightThrough)
			{
				var edge = new HalfEdge(segment.Start);
				var twin = new HalfEdge(segment.End);
				edge.LinkTwin(twin);
				AddEdgeToMap(segment.End, segment.Start, edge);
				AddEdgeToMap(segment.Start, segment.End, twin);
			}

			HalfEdge? FindEdgeIfExists(Point2D source, Point2D target)
			{
				if (!halfEdges.TryGetValue(source, out var targetToEdge)) return null;
				return targetToEdge.TryGetValue(target, out var edge) ? edge : null;
			}

			void AddEdgeToMap(Point2D source, Point2D target, HalfEdge edge)
			{
				if (halfEdges.TryGetValue(source, out var targetsToEdges))
				{
					if (!targetsToEdges.ContainsKey(target)) targetsToEdges.Add(target, edge);

					return;
				}
				halfEdges.Add(source, new Dictionary<Point2D, HalfEdge>());
				halfEdges[source].Add(target, edge);
			}
			
			// Construct HalfEdges and vertices
			foreach (var target in edgesOnVertex.Keys)
			{
				var edges = edgesOnVertex[target].ToList();
				edges.Sort(new RadialLineSegmentComparer());
				var edgeCount = edges.Count;
				HalfEdge? previousTwin = null;
				HalfEdge? firstEdgeIn = null;
				for (var i = 0; i < edgeCount; i++)
				{
					var line = edges[i];
					var edgeIn = FindEdgeIfExists(line.End, target) ?? new HalfEdge(target);
					var edgeInTwin = FindEdgeIfExists(target, line.End) ?? new HalfEdge(line.End);
					
					edgeIn.LinkTwin(edgeInTwin);
					AddEdgeToMap(line.End, target, edgeIn);
					AddEdgeToMap(target, line.End, edgeInTwin);

					if (previousTwin is not null) edgeIn.LinkNext(previousTwin);
					previousTwin = edgeInTwin;

					firstEdgeIn ??= edgeIn;
				}

				if (previousTwin != null) firstEdgeIn?.LinkNext(previousTwin);
			}
			
			// Center of bounding box.
			var center = new Point2D((upperLeft.X + lowerRight.X) / 2, (upperLeft.Y + lowerRight.Y) / 2);

			var mappedSegmentsStraightThrough = segments.Where(s => IsPointOnBoundingBox(s.Start) && IsPointOnBoundingBox(s.End)).SelectMany(s =>
				new [] { Tuple.Create<Point2D, LineSegment2D?>(s.Start, new LineSegment2D(s.End, s.Start)), Tuple.Create<Point2D, LineSegment2D?>(s.End, s) });

			// Get all edges pointing towards oblivion
			var exteriorPointsWithEdge = halfEdges.Values.SelectMany(d => d.Values)
				.Where(edge => edge.Segment is not null && edge.Next is null && edge.Previous is not null)
				.Select(edge => Tuple.Create(edge.TargetVertex.Point, edge.Segment))
				.Union(new []{Tuple.Create<Point2D, LineSegment2D?>(upperLeft, null), Tuple.Create<Point2D, LineSegment2D?>(upperRight, null),
					Tuple.Create<Point2D, LineSegment2D?>(lowerRight, null), Tuple.Create<Point2D, LineSegment2D?>(lowerLeft, null) })
				.Union(mappedSegmentsStraightThrough)
				.ToList();

			// Sort points radially around center of bounding box.
			exteriorPointsWithEdge.Sort((x, y) =>
			{
				var angleX = Utils.CalculateAngle(new LineSegment2D(center, x.Item1));
				var angleY = Utils.CalculateAngle(new LineSegment2D(center, y.Item1));

				return Math.Sign(angleY - angleX);
			});

			// Construct and link exterior edges.
			var exteriorPointCount = exteriorPointsWithEdge.Count;

			HalfEdge? firstExterior = null;
			HalfEdge? previousInterior = null;
			HalfEdge? previousExterior = null;
			for (var i = 0; i < exteriorPointCount; i++)
			{
				var (source, edgeTargetingSource) = exteriorPointsWithEdge[i];
				var (target, edgeTargetingTarget) = exteriorPointsWithEdge[(i + 1) % exteriorPointCount];

				var edge = new HalfEdge(target);
				var twin = new HalfEdge(source);
				
				edge.LinkTwin(twin);
				AddEdgeToMap(source, target, edge);
				AddEdgeToMap(target, source, twin);

				if (edgeTargetingSource is not null)
				{
					var halfEdgeTargetingSource = FindEdgeIfExists(edgeTargetingSource.Start, edgeTargetingSource.End);
					if (halfEdgeTargetingSource?.Next is null)
						halfEdgeTargetingSource?.LinkNext(edge);
				}

				if (edgeTargetingTarget is not null)
				{
					var halfEdgeTargetingTarget = FindEdgeIfExists(edgeTargetingTarget.Start, edgeTargetingTarget.End);
					if (halfEdgeTargetingTarget is not null)
					{
						edge.LinkNext(halfEdgeTargetingTarget.Twin!);
					}
				}

				if (previousInterior is not null && previousInterior.Next is null)
				{
					previousInterior.LinkNext(edge);
				}

				if (previousExterior is not null)
				{
					twin.LinkNext(previousExterior);
				}

				firstExterior ??= twin;

				previousInterior = edge;
				previousExterior = twin;
			}
			
			if (previousInterior?.Next is null) previousInterior?.LinkNext(firstExterior?.Twin);
			if (previousExterior is not null) firstExterior?.LinkNext(previousExterior);
			
			// Create faces
			var edgeQueue = new Queue<Tuple<HalfEdge, bool>>();
			var visited = new HashSet<Tuple<Point2D?, Point2D>>();
			
			edgeQueue.Enqueue(Tuple.Create(halfEdges.Values.First().Values.First(), true));

			// Function to construct the visited key from a half edge.
			Tuple<Point2D?, Point2D> ConstructVisitedKey(HalfEdge edge)
			{
				return Tuple.Create(edge.SourceVertex?.Point, edge.TargetVertex.Point);
			}
			
			// Check if this face has already been visited.
			bool IsFaceVisited(HalfEdge start)
			{
				var current = start;
				while (true)
				{
					var key = ConstructVisitedKey(current!);
					if (visited.Contains(key))
					{
						return true;
					}

					if (current!.Next is null)
					{
						Console.WriteLine($"Next of {current} is null");
					}
					
					current = current!.Next;
					if (current?.Equals(start) ?? false) return false;
				}
			}

			// BFS (kind of) the DCEL to construct faces.
			while (edgeQueue.Count > 0)
			{
				var (current, checkFace) = edgeQueue.Dequeue();
				
				var key = ConstructVisitedKey(current);
				if (visited.Contains(key)) continue;

				if (current.Next is null)
				{
					Console.WriteLine($"Next is null on {current}");
				}
				
				edgeQueue.Enqueue(Tuple.Create(current.Next!, false));
				edgeQueue.Enqueue(Tuple.Create(current.Twin!, true));
				
				if (checkFace && !IsFaceVisited(current))
				{
					var newFace = new Face(current);
					// Don't add external faces to the DCEL.
					if (!newFace.IsFaceExternal())
						faces.Add(new Face(current));
					else
					{
						// Console.WriteLine("Skipped adding an external face");
					}
				}

				visited.Add(key);

			}

			foreach (var face in faces)
			{
				face.LinkEdges();
			}
			
			// Console.WriteLine($"There are {faces.Count} faces in the DCEL");

			var vertices = new Dictionary<Point2D, Vertex>();
			foreach (var halfEdge in halfEdges.Values.SelectMany(d => d.Values))
			{
				var targetPoint = halfEdge.TargetVertex.Point;
				var target = halfEdge.TargetVertex;
				if (vertices.ContainsKey(targetPoint))
				{
					halfEdge.TargetVertex = vertices[targetPoint];
				}
				else
				{
					vertices.Add(targetPoint, target);
				}
			}

			return new Dcel(vertices, halfEdges, faces);
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
				var msg = $"Could not find vertex {v2} in the DCEL.";
				Debug.WriteLine(msg, "Warn");
				throw new VertexNotFoundException(msg);
			}

			// Find a valid half edge.
			var h1 = existingVertex.GetIncidentEdges()
				.Where(edge => edge.TargetVertex.Equals(existingVertex) && edge.IncidentFace is not null)
				.FirstOrDefault(edge => edge.IncidentFace?.ContainsPoint(v1) ?? false);

			if (h1 is null)
			{
				var msg = $"Could not find an edge connected to {v2} with a " + $"face that contains {v1}";
				Debug.WriteLine(msg, "Warn");
				throw new InvalidOperationException(msg);
			}

			var h2 = h1.Next;
			if (h2 is null)
			{
				var msg = $"HalfEdge {h1} has no next edge in DCEL. This is bad!";
				Debug.WriteLine(msg, "Error");
				throw new CorruptedDcelException(msg);
			}

			var h3 = new HalfEdge(v1);
			var h3Twin = new HalfEdge(existingVertex);

            // Update prev, next, and twins
            h1.LinkNext(h3);
            h3.LinkNext(h3Twin);
			h3Twin.LinkNext(h2);
			h3.LinkTwin(h3Twin);

			// Update incident faces
			h3.IncidentFace = h1.IncidentFace;
			h3Twin.IncidentFace = h2.IncidentFace;

			// Update DCEL fields.
			AddEdgeToMap(h3);
			AddEdgeToMap(h3Twin);
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
			// Establish pointers
			var h = GetEdge(source, target);
			if (h is null)
				throw new HalfEdgeNotFoundException($"Could not find half edge between {source} and {target}");
			var hTwin = h.Twin;
			var hNext = h.Next;
			var hTwinPrevious = hTwin?.Previous;

			if (hTwin is null || hNext is null || hTwinPrevious is null)
				throw new CorruptedDcelException($"Edges around {h} are corrupted.");
			
			var hTarget = h.TargetVertex;
			
			// Create new edges
			var newEdge = new HalfEdge(hTarget);
			var newEdgeTwin = new HalfEdge(v);
			// HalfEdge constructor creates the vertex object.
			var newVertex = newEdgeTwin.TargetVertex;
			
			// Update target vertices
			// h.TargetVertex = newVertex;
			UpdateEdgeTarget(h, newVertex);
			
			// Update forward paths
			h.LinkNext(newEdge);
			newEdge.LinkNext(hNext);
			hTwinPrevious.LinkNext(newEdgeTwin);
			newEdgeTwin.LinkNext(hTwin);
			
			// Update twins
			newEdge.LinkTwin(newEdgeTwin);
			
			// Update Faces
			newEdge.IncidentFace = h.IncidentFace;
			newEdgeTwin.IncidentFace = hTwin.IncidentFace;
			
			// Add new edges to the DCEL
			AddEdgeToMap(newEdge);
			AddEdgeToMap(newEdgeTwin);
			
			// Add new vertex to map
			Vertices.Add(v, newVertex);
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
			// Find an edge going from v, say hv, and an edge going from u, say hu, such that the incident faces
			// are the same. Must also make sure that the line between the two points does not intersect any edge of the
			// face.
			HalfEdge? hv = null, hu = null, hvPrev = null, huPrev = null;
			Face? originalFace = null;
			if (!HalfEdges.ContainsKey(v) || !HalfEdges.ContainsKey(u))
				throw new VertexNotFoundException($"DCEL does not contain both v ({v}) and u ({u})");

			void FindPointers()
			{
				foreach (var hvCandidate in HalfEdges[v].Values)
				{
					foreach (var huCandidate in HalfEdges[u].Values
						         .Where(huCandidate => huCandidate.IncidentFace?.Equals(hvCandidate.IncidentFace) ?? false))
					{
						originalFace = huCandidate.IncidentFace;
						var candidateSegment = new LineSegment2D(v, u);
						var canSplit = (originalFace?.GetFaceEdges() ?? new List<HalfEdge>())
							.Select(edge => edge.Segment?.IntersectionWith(candidateSegment))
							.Where(intersection => intersection is not null)
							.All(intersection => intersection!.Equals(v) || intersection.Equals(u));

						if (!canSplit) continue;
						
						hv = hvCandidate;
						hvPrev = hv.Previous;
						hu = huCandidate;
						huPrev = hu.Previous;
						return;
					}
				}
			}
			
			FindPointers();
			if (new object?[] { hv, hu, hvPrev, huPrev, originalFace }.Any(elem => elem is null))
			{
				throw new InvalidOperationException($"Cannot split a face between {v} and {u}");
			}

			// Create new edges
			var newEdge = new HalfEdge(u);
			var newEdgeTwin = new HalfEdge(v);
			
			// Link edges
			hvPrev!.LinkNext(newEdge);
			newEdge.LinkNext(hu!);
			huPrev!.LinkNext(newEdgeTwin);
			newEdgeTwin.LinkNext(hv!);
			
			// Set twins
			newEdge.LinkTwin(newEdgeTwin);
			
			// Create new faces
			var f1 = new Face(newEdge);
			var f2 = new Face(newEdgeTwin);
			
			// Update edges on face
			f1.LinkEdges();
			f2.LinkEdges();
			
			// Add new edges
			AddEdgeToMap(newEdge);
			AddEdgeToMap(newEdgeTwin);
			
			// Remove old face
			Faces.Remove(originalFace!);
			
			// Add new faces
			Faces.Add(f1);
			Faces.Add(f2);
			
			return Tuple.Create(f1, f2);
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
			Console.WriteLine("Drawing DCEL");
			var colors = new[] {
				"#cc6666", "#997a00", "#00f261", "#408cff", "#584359", "#d9a3a3", "#bfb68f",
				"#4d665a", "#1a3866", "#f200c2", "#4c1400", "#f2e63d", "#1d7356", "#bfd9ff",
				"#ffbff2", "#f26d3d", "#57661a", "#bffff2", "#3939e6", "#ff408c", "#664733",
				"#bfff40", "#00f2e2", "#1b134d", "#731d3f", "#995200", "#eaffbf", "#2daab3",
				"#7453a6", "#b20018", "#f29d3d", "#12330d", "#004759", "#bf40ff", "#330007",
				"#4c3913", "#65b359", "#0077b3"
			};

			const string vertexColor = "#530059";

            const double epsilon = 0.01;
            var i = 0;
            foreach (var face in Faces)
            {
	            var centroid = face.Centroid();
	            var color = colors[i % colors.Length];
	            foreach (var edge in face.GetFaceEdges())
	            {
		            var source = edge.SourceVertex!.Point;
		            var target = edge.TargetVertex.Point;
		            DrawSegment(source, target, centroid, epsilon, color, canvas);
	            }
	            i += 1;
            }
            
            canvas.StrokeColor = Colors.White;
            canvas.FillColor = Colors.White;
            foreach (var pf in Vertices.Keys.Select(v => new PointF((float)v.X, (float)v.Y)))
            {
	            canvas.FillCircle(pf, 6);
            }
		}

		private static void DrawSegment(Point2D source, Point2D target, Point2D centroid,
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
		/// <param name="epsilon">The shift scale.</param>
		/// <returns>A point along the line p1 -> p2 scaled by epsilon.</returns>
		private static Point2D ShiftPointTowardPoint(Point2D p1, Point2D p2, double epsilon)
		{
			var dx = p2.X - p1.X;
			var dy = p2.Y - p1.Y;
			if (dx == 0)
				return new Point2D(p1.X, p1.Y + (Math.Sign(dy) * epsilon));

			return dy == 0 ? new Point2D(p1.X + (Math.Sign(dx) * epsilon), p1.Y) : 
				new Point2D(p1.X + (dx * epsilon), p1.Y + (dy * epsilon));
		}
	}
}

