using System;
namespace VoronoiModel.Geometry
{
	public class Utils
	{
		/// <summary>
		/// The tolerance for rounding errors when checking equality.
		/// </summary>
		public static readonly double EPSILON = 0.000000001;

		/// <summary>
		/// A primitive, but potentially useful equality check for arithmetic
		/// that may be affected by rounding errors.
		/// </summary>
		/// <param name="a">The first double.</param>
		/// <param name="b">The second double.</param>
		/// <returns></returns>
		public static bool AreClose(double a, double b)
		{
			return Math.Abs(a - b) < EPSILON;
		}
	}
}

