using System;
namespace VoronoiModel.Geometry
{
	public class LineSegment2D
	{
		public Point2D Start { get; }
		public Point2D End { get; }

		public LineSegment2D(Point2D start, Point2D end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// A helper method to solve the linear equations for the parametric
		/// intersection.
		/// </summary>
		/// <param name="other">The other segment.</param>
		/// <returns>The vector representing the parameters of the intersection point.</returns>
		private Vector? SolveIntersection(LineSegment2D other)
		{
            var A = new Matrix2x2(End.X - Start.X, other.Start.X - other.End.X,
                End.Y - Start.Y, other.Start.Y - other.End.Y);
            var b = new Vector(other.Start.X - Start.X, other.Start.Y - Start.Y);

            return A.Inverse()?.Multiply(b);
        }

		/// <summary>
		/// A helper method to check that the parameterized point is within the bounds
		/// of the line (0 <= p <= 1).
		/// </summary>
		/// <param name="parameters">The parameters involved in the intersection.</param>
		/// <returns>True if all passes parameters fall in the bounds.</returns>
		private static bool IsParametricPointOn(params double[] parameters)
		{
            foreach(var param in parameters)
			{
				if (param < 0 || param > 1)
				{
					return false;
				}
			}

			return true;
        }

		/// <summary>
		/// Find the point where this segment intersects another one.
		/// </summary>
		/// <param name="other">The other line segment.</param>
		/// <returns>The point where the two segments intersect. Or null if they do not.</returns>
		public Point2D? IntersectionWith(LineSegment2D other)
		{
			var x = SolveIntersection(other);
			if (x is null) return null;

			var s = x.Get(0);
			var t = x.Get(1);

			if (!IsParametricPointOn(s, t)) return null;

			var intersection = new Point2D(
				((1 - s) * Start.X) + (s * End.X),
				((1 - s) * Start.Y) + (s * End.Y)
			);
			return intersection;
		}

		/// <summary>
		/// Check if another line segment intersects with this one.
		/// </summary>
		/// <param name="other">The other line segment.</param>
		/// <returns>True if the segments intersect.</returns>
		public bool IntersectsWith(LineSegment2D other)
		{
			var x = SolveIntersection(other);
			return (x is not null && IsParametricPointOn(x.Get(0), x.Get(1)));
		}

		/// <summary>
		/// Check if this line segment intersects with a horizontal line passing
		/// through the specified intercept.
		/// </summary>
		/// <param name="intercept">The y-intercept of a horizontal line.</param>
		/// <returns>True if the line intersects with this segment.</returns>
		public bool IntersectsWith(double intercept)
		{
			var otherStart = new Point2D(0, intercept);
			var otherEnd = new Point2D(1, intercept);
			var x = SolveIntersection(new LineSegment2D(otherStart, otherEnd));

			// Here we only care that the parameter for this segment is between
			// 0 and 1. 
			if (x is null || !IsParametricPointOn(x.Get(0))) return false;

			return true;
		}

		public Point2D? Evaluate(decimal x)
		{
			throw new NotImplementedException();
		}

		public bool IsPointOn(Point2D point)
		{
			throw new NotImplementedException();
		}
	}
}

