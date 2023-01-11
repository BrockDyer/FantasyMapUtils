using System;
using Microsoft.Maui.Graphics;

namespace VoronoiModel
{
	public class VoronoiPoint
	{
		/// <summary>
		/// An optional name of the voronoi point.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// The x value of the point. This should only be set within the
		/// algorithm.
		/// </summary>
		internal double? X;

		/// <summary>
		/// Get this point's x value.
		/// </summary>
		/// <returns>A double representing the x-coord of the point.</returns>
        public double? GetX()
        {
            return X;
        }

        /// <summary>
        /// The y value of the point. This should only be set within the
        /// algorithm.
        /// </summary>
        internal double? Y;

		/// <summary>
		/// Get this point's y value.
		/// </summary>
		/// <returns>A double representing the y-coord of the point.</returns>
		public double? GetY()
		{
			return Y;
		}

		public static readonly double DEFAULT_WEIGHT = 0;
		private double weight = DEFAULT_WEIGHT;

		/// <summary>
		/// The optional weight of the voronoi point.
		/// </summary>
		public double? Weight
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

