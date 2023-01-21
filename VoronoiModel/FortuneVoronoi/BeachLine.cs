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
    private readonly List<Point2D> _beachLine;
    
    /// <summary>
    /// Get the number of elements in the beach line.
    /// </summary>
    public int Count => _beachLine.Count;

    public BeachLine(Point2D initialPoint)
    {
        _beachLine = new List<Point2D> { initialPoint };
    }

    /// <summary>
    /// Compute the breakpoint between two points given the y coordinate of the sweep line. 
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    /// <param name="y">The y value of the sweep line.</param>
    /// <returns>The center of the circle tangent to the sweep line that passes through p1 and p2.</returns>
    public static Point2D ComputeBreakpoint(Point2D p1, Point2D p2, double y)
    {
        // https://www.desmos.com/calculator/fsp6oy3lh0
        // Find the intersection between the perpendicular bisector of p1 and p2, and the parabola defined by
        // p1 as a focus and y as a directrix.
        // Special thanks to Richard Catalano Jr. for helping me through the geometry of the problem.
        
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
        var cxRight = (-b + sqrtPart) / 2;
        var cxLeft = (-b - sqrtPart) / 2;

        return p1.X < p2.X ? new Point2D(cxLeft, ComputeCy(cxLeft)) : new Point2D(cxRight, ComputeCy(cxRight));
    }
    
    /// <summary>
    /// Determine the arc that a new site falls under.
    /// </summary>
    /// <param name="y">The y coordinate of the sweep line.</param>
    /// <param name="newSite">The point that represents the new site.</param>
    /// <returns>The entry in the beach line that is directly above the new site.</returns>
    public BeachLineEntry Search(double y, Point2D newSite)
    {
        var start = 0;
        var end = _beachLine.Count - 1;

        while (true)
        {
            var middle = (int)Math.Round((start + end) / 2d);
            
            if (middle == start || middle == end)
            {
                return new BeachLineEntry(middle, _beachLine[middle]);
            }
            
            var middleArc = _beachLine[middle];

            var leftArc = _beachLine[middle - 1];
            var rightArc = _beachLine[middle + 1];

            var leftBreakpoint = ComputeBreakpoint(leftArc, middleArc, y);
            var rightBreakpoint = ComputeBreakpoint(middleArc, rightArc, y);

            // If the new site is between the two breakpoints, it is under the current arc.
            if (leftBreakpoint.X <= newSite.X && rightBreakpoint.X >= newSite.X)
            {
                return new BeachLineEntry(middle, middleArc);
            }

            // Determine which way to search next.
            if (newSite.X < leftBreakpoint.X)
            {
                end = middle;
            }
            else
            {
                start = middle;
            }
        }
    }

    /// <summary>
    /// Insert the new arc pi within an existing arc pj.
    /// </summary>
    /// <param name="existingArc">The existing arc to split.</param>
    /// <param name="newArc">The new arc to add to the beach line.</param>
    /// <returns>The new entry in the beach line.</returns>
    public BeachLineEntry InsertAndSplit(BeachLineEntry existingArc, Point2D newArc)
    {
        _beachLine.Insert(existingArc.Index + 1, newArc);
        _beachLine.Insert(existingArc.Index + 2, existingArc.Site);

        return new BeachLineEntry(existingArc.Index + 1, newArc);
    }

    /// <summary>
    /// Given a reference to an entry on the beach line, delete it.
    /// </summary>
    /// <param name="entry">The entry to delete.</param>
    public void Delete(BeachLineEntry entry)
    {
        _beachLine.RemoveAt(entry.Index);
    }

    public override string ToString()
    {
        return "<" + string.Join(", ", _beachLine) + ">";
    }
}