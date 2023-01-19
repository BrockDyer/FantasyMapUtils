using VoronoiModel.Geometry;

namespace VoronoiModel.FortuneVoronoi;

public class Event
{
    private Point2D location;
    public bool IsValid { get; internal set; }

    public Event(Point2D location)
    {
        this.location = location;
    }
}

public class SiteEvent : Event
{
    public Point2D Site { get; }
    public SiteEvent(Point2D location, Point2D site) : base(location)
    {
        Site = site;
    }
}

public class EdgeIntersectionEvent : Event
{
    
    public EdgeIntersectionEvent(Point2D location) : base(location)
    {
        
    }
}