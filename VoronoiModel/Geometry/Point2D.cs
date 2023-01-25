namespace VoronoiModel.Geometry
{
	public class Point2D : IEquatable<Point2D>
	{
		public double X { get; }
		public double Y { get; }

		public Point2D(double x, double y)
		{
			X = x;
			Y = y;
		}

        public override bool Equals(object? obj)
        {
            return obj is Point2D d &&
                   Utils.AreClose(X, d.X) &&
                   Utils.AreClose(Y, d.Y);
        }

        public override int GetHashCode()
        {
	        return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public bool Equals(Point2D? other)
        {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return X.Equals(other.X) && Y.Equals(other.Y);
        }
	}
}

