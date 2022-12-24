using System;
using Microsoft.Maui.Graphics;

namespace VoronoiModel
{
	public class VoronoiPoint
	{
		/// <summary>
		/// An optional name of the voronoi point.
		/// </summary>
		public string? Name;

		/// <summary>
		/// The x value of the point. This should only be set within the
		/// algorithm.
		/// </summary>
		Decimal? X;

		/// <summary>
		/// The y value of the point. This should only be set within the
		/// algorithm.
		/// </summary>
		Decimal? Y;

		public static readonly Decimal DEFAULT_WEIGHT = 0;
		private Decimal weight = DEFAULT_WEIGHT;

		/// <summary>
		/// The optional weight of the voronoi point.
		/// </summary>
		public Decimal? Weight
        {
            get
            {
                if (weight.Equals(DEFAULT_WEIGHT))
                {
                    return null;
                }
                return weight;
            }
			set => weight = value.GetValueOrDefault(DEFAULT_WEIGHT);
        }

		/// <summary>
		/// The color of the voronoi point. Used for visual representations. The
		/// default is Black.
		/// </summary>
        public Color Color = Colors.Black;

        public override string ToString()
        {
			string output = "";
			output += Name ?? "Vornoi Point";
			if (X is not null && Y is not null)
			{
				output += string.Format(" at ({0}, {1})", X, Y);
			}

			if (Weight is not null)
			{
				output += string.Format(" with weight {0}", Weight);
			}

			return output;
        }
    }
}

