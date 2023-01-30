using System.Diagnostics;
using VoronoiModel.Geometry;
using VoronoiModel.PlanarSubdivision;

namespace VoronoiModel.FortuneVoronoi;

public static class Algorithm
{
    /// <summary>
    /// Compute a voronoi diagram from a given collection of voronoi sites contained within a given bounding box.
    /// </summary>
    /// <param name="upperLeft">The upper left corner of the bounding box.</param>
    /// <param name="lowerRight">The lower right corner of the bounding box.</param>
    /// <param name="sites">The voronoi sites.</param>
    /// <returns>A doubly connected edge list that represents the planar subdivision (Voronoi Diagram)</returns>
    public static Dcel ComputeVoronoi(Point2D upperLeft, Point2D lowerRight, IEnumerable<Point2D> sites)
    {
        var eventQueue = new PriorityQueue<Event, double>();

        var triplesToEvents = new Dictionary<Tuple<Point2D, Point2D, Point2D>, VertexEvent>();
        
        // Store the bisectors as <pa, pb> -> segment
        // We will use these to construct the DCEL at the end.
        var bisectors = new Dictionary<Tuple<Point2D, Point2D>, Bisector>();

        // Add site events to queue.
        foreach (var p in sites)
        {
            var siteEvent = new SiteEvent(p);
            eventQueue.Enqueue(siteEvent, p.Y);
        }

        var firstSite = eventQueue.Dequeue();
        var beachLine = new BeachLine(firstSite.Location);

        // While the queue is not empty, process events
        while (eventQueue.Count > 0)
        {
            var currentEvent = eventQueue.Dequeue();
            
            // Skip invalid events.
            if (!currentEvent.IsValid) continue;
            
            var sweepLine = currentEvent.Location.Y;
            Debug.WriteLine($"Sweep Line at: {sweepLine}");

            switch (currentEvent)
            {
                // Handle Site event
                case SiteEvent:
                {
                    // Find the arc above the new site.
                    var arcAbove = beachLine.Search(sweepLine, currentEvent.Location);
                    var index = arcAbove.Index;

                    // Invalidate any vertex event involving arcAbove as the middle node.
                    if (index - 1 >= 0 && index + 1 < beachLine.Count)
                    {
                        var triple = Tuple.Create(beachLine.Get(index - 1), beachLine.Get(index),
                            beachLine.Get(index + 1));
                        if (triplesToEvents.TryGetValue(triple, out var value))
                        {
                            value.IsValid = false;
                        }
                        triplesToEvents.Remove(triple);
                    }
                    
                    // Add the new arc.
                    var newEntry = beachLine.InsertAndSplit(arcAbove, currentEvent.Location);
                
                    // Create dangling edge in the planar subdivision
                    var bisectorKey = Tuple.Create(arcAbove.Site, currentEvent.Location);
                    var midpoint = new Point2D((arcAbove.Site.X + currentEvent.Location.X) / 2,
                        (arcAbove.Site.Y + currentEvent.Location.Y) / 2);
                    bisectors.Add(bisectorKey, new Bisector(midpoint));
                
                    // Compute future vertex events events
                    // We only need to check triples affected by the split.
                    for (var i = newEntry.Index - 2; i < newEntry.Index + 3; i++)
                    {
                        // Keep i within bounds of the beach line. 
                        // We also don't need to compute for pj, pi, pj
                        if (i < 0 || i + 1 == newEntry.Index) continue;
                        if (i + 2 >= beachLine.Count) break;

                        var triple = Tuple.Create(beachLine.Get(i), beachLine.Get(i + 1), beachLine.Get(i + 2));

                        var circumcircle = Utils.ComputeCircle(triple);
                        var top = Utils.GetTopOfCircle(circumcircle);
                
                        // Create vertex event if the top of the circle is above the sweep line. 
                        if (!(top.Y > sweepLine)) continue;
                        var newVertexEvent = new VertexEvent(top, triple);
                        triplesToEvents.Add(triple, newVertexEvent);
                        eventQueue.Enqueue(newVertexEvent, newVertexEvent.Location.Y);
                    }

                    break;
                }
                // Handle vertex event (aka circle event).
                case VertexEvent vertexEvent:
                    var pj = beachLine.FindArcInMiddle(vertexEvent.Triple, sweepLine);
                    
                    // Delete any events that arose from triples involving pj.
                    for (var i = pj.Index - 2; i < pj.Index + 3; i++)
                    {
                        if (i < 0) continue;
                        if (i + 2 >= beachLine.Count) break;

                        var left = beachLine.Get(i);
                        var center = beachLine.Get(i + 1);
                        var right = beachLine.Get(i + 2);
                        var triple = Tuple.Create(left, center, right);
                        if (triplesToEvents.TryGetValue(triple, out var value))
                        {
                            value.IsValid = false;
                        }

                        triplesToEvents.Remove(triple);
                    }
                         
                    // Delete entry for pj from the beach line.         
                    beachLine.Delete(pj);
                    
                    // Create a new vertex in the diagram at the circumcenter of pi, pj, and pk 
                    // Join the two edges for the bisectors of (pi, pj) and (pj, pk) to this vertex.
                    var bisectorPiPj = Tuple.Create(vertexEvent.Triple.Item1, vertexEvent.Triple.Item2);
                    var bisectorPjPi = Tuple.Create(vertexEvent.Triple.Item2, vertexEvent.Triple.Item1);
                    var bisectorPjPk = Tuple.Create(vertexEvent.Triple.Item2, vertexEvent.Triple.Item3);
                    var bisectorPkPj = Tuple.Create(vertexEvent.Triple.Item3, vertexEvent.Triple.Item1);
                    
                    if (bisectors.TryGetValue(bisectorPiPj, out var pipjValue))
                    {
                        pipjValue.Connect(vertexEvent.Location);
                    }
                    if (bisectors.TryGetValue(bisectorPjPi, out var pjpiValue))
                    {
                        pjpiValue.Connect(vertexEvent.Location);
                    }
                    if (bisectors.TryGetValue(bisectorPjPk, out var pjpkValue))
                    {
                        pjpkValue.Connect(vertexEvent.Location);
                    }
                    if (bisectors.TryGetValue(bisectorPkPj, out var pkpjValue))
                    {
                        pkpjValue.Connect(vertexEvent.Location);
                    }
                    
                    // Create a dangling edge for the bisector between pi and pk.
                    var bisectorPiPkKey = Tuple.Create(vertexEvent.Triple.Item1, vertexEvent.Triple.Item3);
                    var pi = vertexEvent.Triple.Item1;
                    var pk = vertexEvent.Triple.Item3;
                    var piPkMidpoint = new Point2D((pi.X + pk.X) / 2, (pi.Y + pk.Y) / 2);

                    if (!bisectors.ContainsKey(bisectorPiPkKey))
                        bisectors.Add(bisectorPiPkKey, new Bisector(piPkMidpoint));

                    // Generate the two new events due to the consecutive triples involving pi and pk.
                    var pkIndex = pj.Index; // The index of pk (now that pj has been deleted).
                    if (pkIndex - 2 >= 0)
                    {
                        var newEventTriple1 = Tuple.Create(beachLine.Get(pkIndex - 2), beachLine.Get(pkIndex - 1),
                            beachLine.Get(pkIndex));

                        var newEvent1 = new VertexEvent(Utils.ComputeCircle(newEventTriple1).Item1, newEventTriple1);
                        triplesToEvents.Add(newEventTriple1, newEvent1);
                        eventQueue.Enqueue(newEvent1, newEvent1.Location.Y);
                    }

                    if (pkIndex + 1 < beachLine.Count)
                    {
                        var newEventTriple2 = Tuple.Create(beachLine.Get(pkIndex - 1), beachLine.Get(pkIndex),
                            beachLine.Get(pkIndex + 1));

                        var newEvent2 = new VertexEvent(Utils.ComputeCircle(newEventTriple2).Item1, newEventTriple2);
                        eventQueue.Enqueue(newEvent2, newEvent2.Location.Y);
                        triplesToEvents.Add(newEventTriple2, newEvent2);
                    }
                    
                    break; 
            }
        }
        
        // Confine bisectors to the bounding box.
        var upperRight = new Point2D(lowerRight.X, upperLeft.Y);
        var lowerLeft = new Point2D(upperLeft.X, lowerRight.Y);
        var boundingBox = new LineSegment2D[]
        {
            new LineSegment2D(upperLeft, upperRight),
            new LineSegment2D(upperRight, lowerRight),
            new LineSegment2D(lowerRight, lowerLeft),
            new LineSegment2D(lowerLeft, upperLeft)
        };

        Tuple<double, double, double> GeneralLine(Point2D p1, Point2D p2)
        {
            var a = p1.X - p2.X == 0
                ? 0
                : (p1.Y - p2.Y) / (p1.X - p2.X);
            double b = a == 0 ? 1 : -1;
            var c = a == 0 ? p1.X : a * p1.X - p1.Y;

            return Tuple.Create(a, b, c);
        }
        
        foreach (var bisector in bisectors.Values.Where(bisector => !bisector.IsBounded))
        {
            GeneralLine(bisector.PointA, bisector.OriginalPoint).Deconstruct(out var aBisector, out var bBisector, out var cBisector);
            foreach (var line in boundingBox)
            {
                GeneralLine(line.Start, line.End).Deconstruct(out var aLine, out var bLine, out var cLine);

                var aMatrix = new Matrix2X2(aBisector, bBisector, aLine, bLine);
                var bVector = new Vector(cBisector, cLine);

                var solution = aMatrix.Inverse()?.Multiply(bVector);
                if (solution is null) continue;

                var intersectionPoint = new Point2D(solution.Get(0), solution.Get(1));
                GeneralLine(bisector.PointA, intersectionPoint).Deconstruct(out var mIntersection, out _, out _);
                if (Utils.AreClose(aBisector, mIntersection))
                {
                    bisector.Connect(intersectionPoint);
                }
            }
        }

        // Construct the DCEL from the bisectors.
        var dcel = Dcel.Create(upperLeft, lowerRight, bisectors.Values.Select(b => new LineSegment2D(b.PointA, b.PointB)).ToList());
        return dcel;
    }
}