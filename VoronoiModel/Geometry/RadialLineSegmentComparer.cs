namespace VoronoiModel.Geometry;

public class RadialLineSegmentComparer : IComparer<LineSegment2D>
{
    public int Compare(LineSegment2D? x, LineSegment2D? y)
    {
        switch (x)
        {
            case null when y is null:
                return 0;
            case null:
                return -1;
        }

        if (y is null) return 1;
        
        var angleX = Utils.CalculateAngle(x);
        var angleY = Utils.CalculateAngle(y);

        return Math.Sign(angleY - angleX);
    }
}