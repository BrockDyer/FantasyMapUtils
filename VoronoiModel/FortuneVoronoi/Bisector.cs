using VoronoiModel.Geometry;

namespace VoronoiModel.FortuneVoronoi;

public class Bisector
{
    /// <summary>
    /// Represents the point at infinity.
    /// </summary>
    private class PInfinity : Point2D
    {
        public PInfinity() : base(double.MaxValue, double.MaxValue) {}
    }
        
    private static readonly PInfinity InfinityPoint = new ();
        
    public Point2D PointA { get; private set; }
    public Point2D PointB { get; private set; }
    public Point2D OriginalPoint { get; }
    public bool IsBounded { get; private set; }

    private bool _firstConnected;

    public Bisector(Point2D originalPoint) : this()
    {
        OriginalPoint = originalPoint;
    }

    public Bisector()
    {
        PointA = InfinityPoint;
        PointB = InfinityPoint;

        IsBounded = false;
        OriginalPoint = InfinityPoint;

        _firstConnected = false;
        IsBounded = false;
    }

    /// <summary>
    /// Connect this bisector to an endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint to connect it to.</param>
    /// <exception cref="InvalidOperationException">If both endpoints are already connected.</exception>
    public void Connect(Point2D endpoint)
    {
        //if (IsBounded) throw new InvalidOperationException("All connections are already set");

        if (!_firstConnected)
        {
            PointA = endpoint;
            _firstConnected = true;
        }
        else if (!IsBounded)
        {
            PointB = endpoint;
            IsBounded = true;
        }
    }

    /// <summary>
    /// Limit this bisector to some bounding point.
    /// </summary>
    /// <param name="bound">The bounding point to limit the bisector to.</param>
    /// <exception cref="InvalidOperationException">If neither end is at infinity.</exception>
    public void LimitToBound(Point2D bound)
    {
        if (PointA.Equals(InfinityPoint))
        {
            PointA = bound;
            return;
        }

        if (PointB.Equals(InfinityPoint))
        {
            PointB = bound;
            return;
        }

        throw new InvalidOperationException("Cannot limit bisector that does not have a point at infinity.");
    }

    /// <summary>
    /// Construct the line segment representation of this bisector.
    /// </summary>
    /// <returns>The line segment that represents this bisector.</returns>
    public LineSegment2D GetSegment()
    {
        return new LineSegment2D(PointA, PointB);
    }
}