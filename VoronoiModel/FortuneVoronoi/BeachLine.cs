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
        // The center point of the circle will be on the perpendicular bisector of p1 and p2, it will
        // also be equidistant from p1, p2, and y. Thus we can use the quadratic formula to solve the
        // resulting system of equations.
        
        var a = 2 * (p1.Y - p2.Y) / (p2.X - p1.X);
        var b = 2 * (Math.Pow(p2.Y, 2) - Math.Pow(p1.Y, 2)) / (p2.X - p1.X);
        var c = 
            (Math.Pow(p1.Y, 3) - Math.Pow(p2.Y, 3) - 
                p1.Y * (Math.Pow(p1.X, 2) + Math.Pow(p2.Y, 2) + Math.Pow(p2.X, 2)) + 
                p2.Y * (Math.Pow(p1.X, 2) - Math.Pow(p1.Y, 2) - Math.Pow(p2.X, 2))
            ) / 
                (2 * (p2.X - p1.X));

        var bSquaredMinus4Ac = Math.Sqrt(Math.Pow(b, 2) - 4 * a * c);
        var twoA = 2 * a;
        var cyp = twoA == 0 ? 0 : (-b + bSquaredMinus4Ac) / twoA;
        var cyn = twoA == 0 ? 0 : (-b - bSquaredMinus4Ac) / twoA;

        double cy;
        if (0 <= cyp && cyp < y) cy = cyp;
        else cy = cyn;

        // var r = y - cy;
        var cx = (Math.Pow(p1.Y - cy, 2) + Math.Pow(p2.Y - cy, 2) - Math.Pow(p1.X, 2) + Math.Pow(p2.X, 2)) /
                 (2 * (p2.X - p1.X));

        return new Point2D(cx, cy);
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