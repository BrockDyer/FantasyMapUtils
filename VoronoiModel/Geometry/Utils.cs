using System;
namespace VoronoiModel.Geometry
{
	public static class Utils
	{
		/// <summary>
		/// The tolerance for rounding errors when checking equality.
		/// </summary>
		public const double Epsilon = 0.000000001;

		/// <summary>
		/// A primitive, but potentially useful equality check for arithmetic
		/// that may be affected by rounding errors.
		/// </summary>
		/// <param name="a">The first double.</param>
		/// <param name="b">The second double.</param>
		/// <returns></returns>
		public static bool AreClose(double a, double b)
		{
			return Math.Abs(a - b) < Epsilon;
		}

		/// <summary>
		/// Compute a circle through three points.
		/// </summary>
		/// <param name="points">A triple of points.</param>
		/// <returns>The circle passing through the points.</returns>
		public static Tuple<Point2D, double> ComputeCircle(Tuple<Point2D, Point2D, Point2D> points)
		{
			return ComputeCircle(points.Item1, points.Item2, points.Item3);
		}

		/// <summary>
		/// Compute a circle passing through three points.
		/// </summary>
		/// <param name="p1">The first point.</param>
		/// <param name="p2">The second point.</param>
		/// <param name="p3">The third point.</param>
		/// <returns>A tuple containing the center point and the radius.</returns>
		public static Tuple<Point2D, double> ComputeCircle(Point2D p1, Point2D p2, Point2D p3)
		{
			// Ax = b => x = A'b
			
			var b1 = Math.Pow(p2.X, 2) - Math.Pow(p1.X, 2) + Math.Pow(p2.Y, 2) - Math.Pow(p1.Y, 2);
			var b2 = Math.Pow(p2.X, 2) - Math.Pow(p3.X, 2) + Math.Pow(p2.Y, 2) - Math.Pow(p3.Y, 2);
			var b = new Vector(b1, b2);

			double ComputeMatrixElement(double first, double second)
			{
				return 2 * (first - second);
			}

			var a = new Matrix2X2(ComputeMatrixElement(p2.X, p1.X), ComputeMatrixElement(p2.Y, p1.Y),
				ComputeMatrixElement(p1.X, p3.X), ComputeMatrixElement(p2.Y, p3.Y));

			var solution = a.Inverse()?.Multiply(b);
			if (solution is null)
				throw new InvalidOperationException($"Could not find a circle through {p1}, {p2}, and {p3}");

			var xc = solution.Get(0);
			var yc = solution.Get(1);
			var r = Math.Sqrt(Math.Pow(p1.X - xc, 2) + Math.Pow(p1.Y - yc, 2));
			
			return Tuple.Create(new Point2D(xc, yc), r);
		}

		/// <summary>
		/// Get the top point of a circle.
		/// </summary>
		/// <param name="circle">A tuple containing the center point and the radius of a circle.</param>
		/// <returns>The point on the top of the circle.</returns>
		public static Point2D GetTopOfCircle(Tuple<Point2D, double> circle)
		{
			var center = circle.Item1;
			var radius = circle.Item2;
			return new Point2D(center.X, center.Y + radius);
		}
		
		/// <summary>
		/// Calculate the angle of this line segment.
		/// </summary>
		/// <param name="segment">The line segment to get the angle of.</param>
		/// <returns>The angle of incline.</returns>
		public static double CalculateAngle(LineSegment2D segment)
		{
			var distanceX = segment.End.X - segment.Start.X;
			var distanceY = segment.End.Y - segment.Start.Y;

			return Math.Atan2(distanceY, distanceX);
		}
	}
}

