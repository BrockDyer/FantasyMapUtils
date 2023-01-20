using VoronoiModel.Geometry;

namespace VoronoiModel.FortuneVoronoi;

public class BeachLineEntry
{
    public int Index { get; }
    public Point2D Site { get; }

    public BeachLineEntry(int index, Point2D site)
    {
        Index = index;
        Site = site;
    }
}
public class BeachLine
{
    private string beachline;

    /// <summary>
    /// Compute the breakpoint between two points given the y coordinate of the sweep line. 
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    /// <param name="y">The y value of the sweep line.</param>
    /// <returns>The center of the circle tangent to the sweep line that passes through p1 and p2.</returns>
    public static Point2D ComputeBreakpoint(Point2D p1, Point2D p2, double y)
    {
        var ax = p1.X;
        var ay = p1.Y;
        var bx = p2.X;
        var by = p2.Y;

        double ComputeCy(double cx)
        {
            return (Math.Pow(cx - ax, 2) + Math.Pow(ay, 2) - Math.Pow(y, 2)) / (2 * (ay - y));
        }

        if (Utils.AreClose(ay - by, 0))
        {
            var cx = (ax + bx) / 2;
            return new Point2D(cx, ComputeCy(cx));
        }

        var m = (ay - by) / (ax - bx);
        var mb = -1 / m;
        var py = (ay + by) / 2;
        var px = (ax + bx) / 2;

        var b = -2 * (ax + (ay - y) * mb);
        var c = Math.Pow(ax, 2) + Math.Pow(ay, 2) - Math.Pow(y, 2) - 2 * (ay - y) * (py - mb * px);

        var sqrtPart = Math.Sqrt(Math.Pow(b, 2) - 4 * c);
        var cx1 = (-b + sqrtPart) / 2;
        var cx2 = (-b - sqrtPart) / 2;

        var leftBound = Math.Min(ax, bx);
        var rightBound = Math.Max(ax, bx);

        if (cx1 >= leftBound && cx1 <= rightBound)
        {
            return new Point2D(cx1, ComputeCy(cx1));
        }

        if (cx2 >= leftBound && cx2 <= rightBound)
        {
            return new Point2D(cx2, ComputeCy(cx2));
        }

        throw new InvalidOperationException($"No solution for {p1} and {p2} with sweep line at {y}");
    }
    
    /// <summary>
    /// Determine the arc that a new site falls under.
    /// </summary>
    /// <param name="y">The y coordinate of the sweep line.</param>
    /// <param name="newSite">The point that represents the new site.</param>
    /// <returns>The entry in the beach line that is directly above the new site.</returns>
    public BeachLineEntry Search(double y, Point2D newSite)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Insert the new arc pi within an existing arc pj.
    /// </summary>
    /// <param name="existingArc">The existing arc to split.</param>
    /// <param name="newArc">The new arc to add to the beach line.</param>
    /// <returns>The new entry in the beach line.</returns>
    public BeachLineEntry InsertAndSplit(Point2D existingArc, Point2D newArc)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Given a reference to an entry on the beach line, delete it.
    /// </summary>
    /// <param name="entry">The entry to delete.</param>
    public void Delete(BeachLineEntry entry)
    {
        throw new NotImplementedException();
    }
}