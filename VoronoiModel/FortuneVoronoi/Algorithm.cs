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
        var result = Dcel.Create(upperLeft, lowerRight);

        var triplesToEvents = new Dictionary<Tuple<Point2D, Point2D, Point2D>, VertexEvent>();

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

            switch (currentEvent)
            {
                // Handle Site event
                case SiteEvent:
                {
                    // Find the arc above the new site.
                    var arcAbove = beachLine.Search(sweepLine, currentEvent.Location);
                    
                    // Invalidate any vertex event involving arcAbove as the middle node.
                    if (beachLine.Count >= 3)
                    {
                        var index = arcAbove.Index;
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
                    // TODO
                
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
                        if (i >= beachLine.Count) break;

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
                    // TODO
                    
                    // Create a dangling edge for the bisector between pi and pk.
                    // TODO

                    // Generate the two new events due to the consecutive triples involving pi and pk.
                    var pkIndex = pj.Index; // The index of pk (now that pj has been deleted).
                    var newEventTriple1 = Tuple.Create(beachLine.Get(pkIndex - 2), beachLine.Get(pkIndex - 1),
                        beachLine.Get(pkIndex));
                    var newEventTriple2 = Tuple.Create(beachLine.Get(pkIndex - 1), beachLine.Get(pkIndex),
                        beachLine.Get(pkIndex + 1));
                    var newEvent1 = new VertexEvent(Utils.ComputeCircle(newEventTriple1).Item1, newEventTriple1);
                    var newEvent2 = new VertexEvent(Utils.ComputeCircle(newEventTriple2).Item1, newEventTriple2);
                    
                    eventQueue.Enqueue(newEvent1, newEvent1.Location.Y);
                    eventQueue.Enqueue(newEvent2, newEvent2.Location.Y);
                    
                    triplesToEvents.Add(newEventTriple1, newEvent1);
                    triplesToEvents.Add(newEventTriple2, newEvent2);
                    
                    break; 
            }
        }

        return result;
    }
}