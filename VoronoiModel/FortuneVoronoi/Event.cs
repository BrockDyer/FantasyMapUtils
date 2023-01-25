using VoronoiModel.Geometry;

namespace VoronoiModel.FortuneVoronoi;

internal class Event
{
    /// <summary>
    /// The location of the event.
    /// </summary>
    public Point2D Location { get; }
    /// <summary>
    /// A flag if the event is still valid or not.
    /// </summary>
    public bool IsValid { get; internal set; }

    protected Event(Point2D location)
    {
        Location = location;
        IsValid = true;
    }
}

/// <summary>
/// The event that occurs at a new voronoi site.
/// </summary>
internal class SiteEvent : Event
{
    public SiteEvent(Point2D location) : base(location) {}
}

/// <summary>
/// The event that occurs when three arcs on the beach line squeeze together to form a voronoi edge.
/// </summary>
internal class VertexEvent : Event
{
    public Tuple<Point2D, Point2D, Point2D> Triple { get; }

    public VertexEvent(Point2D location, Tuple<Point2D, Point2D, Point2D> triple) : base(location)
    {
        Triple = triple;
    }
}