using System;
namespace VoronoiModel.Geometry
{
	public class Point2D
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
                   X == d.X &&
                   Y == d.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}

