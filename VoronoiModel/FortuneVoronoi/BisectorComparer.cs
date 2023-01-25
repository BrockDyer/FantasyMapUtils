namespace VoronoiModel.FortuneVoronoi;

/// <summary>
/// Credit: https://github.com/Gimly/DoublyConnectedEdgeList/blob/main/tests/Ethereality.DoublyConnectedEdgeList.Tests/TestSegmentComparer.cs
/// </summary>
public class BisectorComparer : IComparer<Bisector>
{
    public int Compare(Bisector? x, Bisector? y)
    {
        switch (x)
        {
            case null when y is null:
                return 0;
            case null:
                return -1;
        }

        if (y is null) return 1;
        
        var angleX = CalculateAngle(x);
        var angleY = CalculateAngle(y);

        return Math.Sign(angleY - angleX);
    }

    private static double CalculateAngle(Bisector segment)
    {
        var distanceX = segment.PointB.X - segment.PointA.X;
        var distanceY = segment.PointB.Y - segment.PointA.Y;

        return Math.Atan2(distanceY, distanceX);
    }
}