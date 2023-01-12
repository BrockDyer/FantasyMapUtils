using System;
namespace VoronoiModel.Geometry
{
	public class LineSegment2D
	{
		private Point2D Start { get; }
		private Point2D End { get; }

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
            var a = new Matrix2X2(End.X - Start.X, other.Start.X - other.End.X,
                End.Y - Start.Y, other.Start.Y - other.End.Y);
            var b = new Vector(other.Start.X - Start.X, other.Start.Y - Start.Y);

            return a.Inverse()?.Multiply(b);
        }

		/// <summary>
		/// A helper method to check that the parameterized point is within the bounds
		/// of the line (0 &lt;= p &lt;= 1).
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
		/// Check if this line segment shares an endpoint with another.
		/// </summary>
		/// <param name="other">The other line segment.</param>
		/// <returns>True if the line segments have a common endpoint.</returns>
		public bool SharesEndpoint(LineSegment2D other)
		{
			return Start.Equals(other.Start) || Start.Equals(other.End) || End.Equals(other.End) ||
			       End.Equals(other.Start);
		}

		/// <summary>
		/// Check if another line segment intersects with this one. An intersection at the endpoints is counted as true.
		/// Equivalent segments are also considered to intersect.
		/// </summary>
		/// <param name="other">The other line segment.</param>
		/// <returns>True if the segments intersect.</returns>
		public bool IntersectsWith(LineSegment2D other)
		{
			// Considering same segments to be intersecting.
			if (Equals(other)) return true;
			if (SharesEndpoint(other)) return true;
			var x = SolveIntersection(other);
			return x is not null && IsParametricPointOn(x.Get(0), x.Get(1));
		}

		/// <summary>
		/// Check if this line segment intersects with a horizontal line passing
		/// through the specified intercept. An intersection at the endpoints is considered true. Equivalent segments
		/// are also considered to intersect.
		/// </summary>
		/// <param name="intercept">The y-intercept of a horizontal line.</param>
		/// <returns>True if the line intersects with this segment.</returns>
		public bool IntersectsWith(double intercept)
		{
			var otherStart = new Point2D(0, intercept);
			var otherEnd = new Point2D(1, intercept);
			var segment = new LineSegment2D(otherStart, otherEnd);
			// Considering same segments to be intersecting.
			if (Equals(segment)) return true;
			if (SharesEndpoint(segment)) return true;
			
			var x = SolveIntersection(segment);

			// Here we only care that the parameter for this segment is between
			// 0 and 1. 
			return x is not null && IsParametricPointOn(x.Get(0));
		}

		/// <summary>
		/// Check if this line segment intersects with a ray sent towards
		/// negative x from the specified point.
		/// </summary>
		/// <param name="point">the point to check.</param>
		/// <returns>True if this segment intersects with the ray.</returns>
		public bool IntersectsWithLeftRayFrom(Point2D point)
		{
			// We subtract 1 to make sure that we don't land exactly on the
			// boundary.
			var minX = Math.Min(Start.X, End.X) - 1;
			var raySegment = new LineSegment2D(point, new Point2D(minX, point.Y));
			return IntersectsWith(raySegment);
		}

		public Point2D? Evaluate(double x)
		{
			double? slope;
			try
			{
				slope = (End.Y - Start.Y) / (End.X - Start.X);
			}
			catch (DivideByZeroException)
			{
				slope = null;
			}

			double? intercept = slope is null
				? null
				: (Utils.AreClose(End.Y - Start.Y, 0) ? null : Start.Y - (slope.Value * Start.X));

			return slope is null ? null : new Point2D(x , slope.Value * x + intercept!.Value);
		}

		/// <summary>
		/// Check if a point is on this line segment. If the point is one of the endpoints, this returns true.
		/// </summary>
		/// <param name="c">The point to check.</param>
		/// <returns>True if this point is on the segment.</returns>
		public bool IsPointOn(Point2D c)
		{
			// We will refer to Start as A and End as B.
			var vecAb = new Vector(Start, End);
			var vecAc = new Vector(Start, c);
			
			// Check that the point c is collinear with the start and end.
			if (!vecAb.Cross2D(vecAc).IsZero()) return false;
			
			var dotAb = vecAb.Dot(vecAb);
			var dotAc = vecAb.Dot(vecAc);

			// Check if the point is on the endpoints
			if (Utils.AreClose(dotAc, 0) || Utils.AreClose(dotAc, dotAb))
			{
				return true;
			}

			// Check if the point is on segment between endpoints.
			return dotAc > 0 && dotAc < dotAb;
		}

		private bool Equals(LineSegment2D other)
		{
			return Start.Equals(other.Start) && End.Equals(other.End);
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((LineSegment2D)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Start, End);
		}
	}
}

